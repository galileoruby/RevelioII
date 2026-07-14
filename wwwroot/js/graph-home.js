(() => {
    const graphContainer = document.getElementById("graph-container");
    const hoverContent = document.getElementById("graph-hover-content");
    const nodePropertiesContent = document.getElementById("graph-node-properties-content");
    const hoverPanel = document.getElementById("graph-hover-panel");
    const emptyState = document.getElementById("graph-empty-state");

    if (!graphContainer || typeof cytoscape === "undefined") {
        return;
    }

    const graphUrl = graphContainer.dataset.graphUrl;
    const highlightClass = "graph-highlight";
    const rootPath = "$";
    const previewEntryCount = 6;
    const largePayloadByteThreshold = 2048;
    const largePayloadTopLevelThreshold = 20;

    const panelState = {
        selectedNodeId: null,
        activeNodeData: null,
        activeRelationships: [],
        showAllRootEntries: false,
        expandedPaths: new Set([rootPath])
    };

    fetch(graphUrl, {
        headers: {
            Accept: "application/json"
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Unable to load graph data.");
            }

            return response.json();
        })
        .then(graph => {
            const nodes = graph.nodes ?? [];
            const edges = graph.edges ?? [];

            if (nodes.length === 0) {
                graphContainer.classList.add("d-none");
                emptyState?.classList.remove("d-none");
                return;
            }

            emptyState?.classList.add("d-none");
            renderGraph(nodes, edges);
            setDefaultHoverMessage(nodes.length, edges.length);
            setDefaultPropertiesMessage();
        })
        .catch(error => {
            console.error("Failed to load or render graph data.", error);
            hoverPanel?.classList.add("graph-panel-error");
            hoverContent.innerHTML = `<p class="graph-panel-empty">Unable to load graph data.</p><p class="graph-panel-muted">${escapeHtml(error?.message ?? "Unknown graph error.")}</p>`;
            if (nodePropertiesContent) {
                nodePropertiesContent.innerHTML = `<p class="graph-panel-empty">Unable to load node properties.</p>`;
            }
        });

    function renderGraph(nodes, edges) {
        const nodeLabelById = new Map(nodes.map(node => [getNodeId(node.id), node.label]));
        const elements = [
            ...nodes.map(node => ({
                data: {
                    id: getNodeId(node.id),
                    nodeId: node.id,
                    label: node.label,
                    type: node.type,
                    properties: node.properties ?? ""
                }
            })),
            ...edges.map(edge => ({
                data: {
                    id: getEdgeId(edge.id),
                    edgeId: edge.id,
                    source: getNodeId(edge.sourceNodeId),
                    target: getNodeId(edge.targetNodeId),
                    label: edge.relationType,
                    relationType: edge.relationType,
                    sourceLabel: edge.sourceLabel ?? nodeLabelById.get(getNodeId(edge.sourceNodeId)) ?? `Node ${edge.sourceNodeId}`,
                    targetLabel: edge.targetLabel ?? nodeLabelById.get(getNodeId(edge.targetNodeId)) ?? `Node ${edge.targetNodeId}`,
                    properties: edge.properties ?? ""
                }
            }))
        ];

        const cy = cytoscape({
            container: graphContainer,
            elements,
            layout: {
                name: "cose",
                animate: true,
                fit: true,
                padding: 90,
                nodeRepulsion: 700000,
                idealEdgeLength: 240,
                edgeElasticity: 80,
                nestingFactor: 0.9
            },
            minZoom: 0.25,
            maxZoom: 2,
            wheelSensitivity: 0.12,
            style: [
                {
                    selector: "node",
                    style: {
                        label: "data(label)",
                        color: "#7dd1ff",
                        "font-size": 11,
                        "font-weight": 700,
                        "text-wrap": "wrap",
                        "text-max-width": 100,
                        "text-valign": "center",
                        "text-halign": "center",
                        width: 92,
                        height: 92,
                        shape: "ellipse",
                        "background-color": "#214c7d",
                        "background-image": "data:image/svg+xml;utf8,<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 100 100'><defs><radialGradient id='g' cx='35%25' cy='28%25' r='70%25'><stop offset='0%25' stop-color='rgba(255,255,255,0.95)'/><stop offset='32%25' stop-color='rgba(255,255,255,0.52)'/><stop offset='68%25' stop-color='rgba(255,255,255,0.10)'/><stop offset='100%25' stop-color='rgba(255,255,255,0)'/></radialGradient></defs><circle cx='50' cy='50' r='50' fill='url(%23g)'/></svg>",
                        "background-fit": "cover",
                        "background-clip": "none",
                        "border-width": 2,
                        "border-color": "#79cfff",
                        "overlay-opacity": 0,
                        "text-outline-width": 0,
                        "shadow-blur": 18,
                        "shadow-color": "rgba(35, 150, 255, 0.25)",
                        "shadow-opacity": 1,
                        "shadow-offset-x": 0,
                        "shadow-offset-y": 0
                    }
                },
                {
                    selector: "edge",
                    style: {
                        label: "data(label)",
                        width: 1.4,
                        color: "#7da9c7",
                        "font-size": 8,
                        "font-weight": 500,
                        "text-background-opacity": 0,
                        "line-color": "#1e3f67",
                        "target-arrow-color": "#1e3f67",
                        "target-arrow-shape": "none",
                        "curve-style": "bezier",
                        "overlay-opacity": 0,
                        opacity: 0.92
                    }
                },
                {
                    selector: `node.${highlightClass}`,
                    style: {
                        opacity: 1,
                        "border-color": "#a4dcff",
                        "border-width": 3,
                        "shadow-blur": 24,
                        "shadow-color": "rgba(125, 209, 255, 0.35)",
                        "shadow-opacity": 1,
                        "shadow-offset-x": 0,
                        "shadow-offset-y": 0,
                        "z-index": 999
                    }
                }
            ]
        });

        cy.ready(() => {
            cy.fit(cy.elements(), 90);
            cy.center();
        });

        cy.on("mouseover", "node", event => {
            const node = event.target;
            if (panelState.selectedNodeId && panelState.selectedNodeId !== node.id()) {
                return;
            }

            hydratePanelForNode(cy, node);
        });

        cy.on("tap", "node", event => {
            const node = event.target;
            panelState.selectedNodeId = node.id();
            hydratePanelForNode(cy, node);
        });

        nodePropertiesContent?.addEventListener("click", event => {
            const toggleAllButton = event.target.closest("[data-toggle-all]");
            if (toggleAllButton) {
                panelState.showAllRootEntries = !panelState.showAllRootEntries;
                renderNodePropertiesPanel();
                return;
            }

            const togglePathButton = event.target.closest("[data-toggle-path]");
            if (!togglePathButton) {
                return;
            }

            const path = togglePathButton.getAttribute("data-toggle-path");
            if (!path) {
                return;
            }

            if (panelState.expandedPaths.has(path)) {
                panelState.expandedPaths.delete(path);
            } else {
                panelState.expandedPaths.add(path);
            }

            renderNodePropertiesPanel();
        });
    }

    function hydratePanelForNode(cy, node) {
        cy.elements().removeClass(highlightClass);
        node.addClass(highlightClass);

        panelState.activeNodeData = {
            label: node.data("label"),
            type: node.data("type"),
            properties: node.data("properties")
        };
        panelState.activeRelationships = node.connectedEdges().map(edge => edge.data());
        panelState.showAllRootEntries = false;
        panelState.expandedPaths = new Set([rootPath]);

        updateHoverPanel(node, panelState.activeRelationships);
        renderNodePropertiesPanel();
    }

    function updateHoverPanel(node, connectedEdges) {
        const relationshipsMarkup = connectedEdges.length === 0
            ? '<p class="graph-panel-empty">This node has no relationships yet.</p>'
            : `<ul class="graph-relationship-list">${connectedEdges.map(edge => {
                const isSource = edge.source === node.id();
                const otherNodeLabel = isSource ? edge.targetLabel : edge.sourceLabel;
                const direction = isSource ? "to" : "from";
                return `<li><span class="graph-relationship-type">${escapeHtml(edge.relationType)}</span> ${direction} <strong>${escapeHtml(otherNodeLabel)}</strong></li>`;
            }).join("")}</ul>`;

        hoverContent.innerHTML = `
            <div class="graph-hover-node-title">${escapeHtml(node.data("label"))}</div>
            <div class="graph-hover-node-meta">Type: ${escapeHtml(node.data("type"))}</div>
            <div class="graph-hover-node-meta">Connections: ${connectedEdges.length}</div>
            ${relationshipsMarkup}
        `;
    }

    function renderNodePropertiesPanel() {
        if (!nodePropertiesContent) {
            return;
        }

        const node = panelState.activeNodeData;
        if (!node) {
            setDefaultPropertiesMessage();
            return;
        }

        const parsedResult = safeParseProperties(node.properties);
        if (parsedResult.state === "empty") {
            nodePropertiesContent.innerHTML = `
                <p class="graph-panel-muted">Node: ${escapeHtml(node.label)}</p>
                <p class="graph-panel-empty">No node properties are available.</p>
            `;
            return;
        }

        if (parsedResult.state === "invalid") {
            nodePropertiesContent.innerHTML = `
                <p class="graph-panel-muted">Node: ${escapeHtml(node.label)}</p>
                <p class="graph-properties-error">Properties are not valid JSON.</p>
                <pre class="graph-properties-raw" aria-label="Invalid node properties payload">${escapeHtml(parsedResult.rawPreview)}</pre>
            `;
            return;
        }

        const payloadSummary = summarizePayload(parsedResult.value, parsedResult.raw);
        const isLargePayload = payloadSummary.rawBytes > largePayloadByteThreshold
            || payloadSummary.topLevelEntries > largePayloadTopLevelThreshold;

        const rootMarkup = renderJsonValue(parsedResult.value, rootPath, {
            isRoot: true,
            isLargePayload
        });

        const toggleAllLabel = panelState.showAllRootEntries
            ? "Show curated view"
            : "Show all";

        const shouldShowToggleAll = payloadSummary.topLevelEntries > previewEntryCount;
        const largeSummaryMarkup = isLargePayload
            ? `<p class="graph-panel-muted">Large payload detected (${payloadSummary.rawBytes} bytes, ${payloadSummary.topLevelEntries} top-level entries). Showing a summarized view first.</p>`
            : "";

        nodePropertiesContent.innerHTML = `
            <div class="graph-properties-meta">
                <p class="graph-panel-muted">Node: ${escapeHtml(node.label)} (${escapeHtml(node.type)})</p>
                <p class="graph-panel-muted">Payload: ${payloadSummary.rawBytes} bytes</p>
            </div>
            ${largeSummaryMarkup}
            <div class="graph-json-tree" role="tree" aria-label="Node properties JSON">
                ${rootMarkup}
            </div>
            ${shouldShowToggleAll ? `<button type="button" class="graph-json-toggle-all" data-toggle-all="true" aria-label="Toggle full property view">${toggleAllLabel}</button>` : ""}
        `;
    }

    function renderJsonValue(value, path, options = {}) {
        const isRoot = options.isRoot === true;
        const isLargePayload = options.isLargePayload === true;

        if (Array.isArray(value)) {
            const shouldCollapse = !isRoot && !panelState.expandedPaths.has(path);
            const headerLabel = `Array(${value.length})`;
            if (shouldCollapse) {
                return renderCollapsedContainer(path, headerLabel);
            }

            const visibleItems = isRoot && !panelState.showAllRootEntries
                ? value.slice(0, previewEntryCount)
                : value;

            return `
                ${renderExpandableHeader(path, headerLabel, true, isRoot)}
                <ul class="graph-json-list">
                    ${visibleItems.map((item, index) => `
                        <li>
                            <span class="graph-json-key">[${index}]</span>
                            ${renderJsonValue(item, `${path}[${index}]`, { isLargePayload })}
                        </li>
                    `).join("")}
                </ul>
            `;
        }

        if (isPlainObject(value)) {
            const keys = Object.keys(value).sort((a, b) => a.localeCompare(b));
            const shouldCollapse = !isRoot && !panelState.expandedPaths.has(path);
            const headerLabel = `Object(${keys.length})`;
            if (shouldCollapse) {
                return renderCollapsedContainer(path, headerLabel);
            }

            const curatedRoot = isRoot && !panelState.showAllRootEntries;
            const visibleKeys = curatedRoot
                ? keys.slice(0, previewEntryCount)
                : keys;

            const rows = visibleKeys.map(key => {
                const keyPath = `${path}.${key}`;
                return `
                    <li>
                        <span class="graph-json-key">${escapeHtml(key)}</span>
                        ${renderJsonValue(value[key], keyPath, { isLargePayload })}
                    </li>
                `;
            }).join("");

            const hiddenCount = keys.length - visibleKeys.length;
            const curatedMessage = curatedRoot && hiddenCount > 0
                ? `<p class="graph-panel-muted">Showing ${visibleKeys.length} of ${keys.length} properties.</p>`
                : "";
            const largeMessage = isRoot && isLargePayload
                ? `<p class="graph-panel-muted">Nested sections start collapsed for readability.</p>`
                : "";

            return `
                ${renderExpandableHeader(path, headerLabel, true, isRoot)}
                ${curatedMessage}
                ${largeMessage}
                <ul class="graph-json-list">
                    ${rows}
                </ul>
            `;
        }

        return renderPrimitiveValue(value);
    }

    function renderExpandableHeader(path, label, expanded, isRoot) {
        if (isRoot) {
            return `<div class="graph-json-root-label">${escapeHtml(label)}</div>`;
        }

        return `<button type="button" class="graph-json-toggle" data-toggle-path="${escapeHtml(path)}" aria-expanded="${expanded ? "true" : "false"}" aria-label="Toggle ${escapeHtml(label)}">${expanded ? "Collapse" : "Expand"} ${escapeHtml(label)}</button>`;
    }

    function renderCollapsedContainer(path, label) {
        return `<button type="button" class="graph-json-toggle" data-toggle-path="${escapeHtml(path)}" aria-expanded="false" aria-label="Expand ${escapeHtml(label)}">Expand ${escapeHtml(label)}</button>`;
    }

    function renderPrimitiveValue(value) {
        if (value === null) {
            return `<span class="graph-json-value graph-json-null">null</span>`;
        }

        switch (typeof value) {
        case "string":
            return `<span class="graph-json-value graph-json-string">"${escapeHtml(value)}"</span>`;
        case "number":
            return `<span class="graph-json-value graph-json-number">${value}</span>`;
        case "boolean":
            return `<span class="graph-json-value graph-json-boolean">${value}</span>`;
        default:
            return `<span class="graph-json-value">${escapeHtml(String(value))}</span>`;
        }
    }

    function safeParseProperties(rawProperties) {
        if (rawProperties === null || rawProperties === undefined) {
            return { state: "empty" };
        }

        const text = String(rawProperties).trim();
        if (text.length === 0) {
            return { state: "empty" };
        }

        try {
            const parsed = JSON.parse(text);
            if (parsed === null) {
                return { state: "empty" };
            }

            return {
                state: "valid",
                value: parsed,
                raw: text
            };
        } catch {
            return {
                state: "invalid",
                rawPreview: text.slice(0, 800)
            };
        }
    }

    function summarizePayload(parsedValue, rawText) {
        const topLevelEntries = Array.isArray(parsedValue)
            ? parsedValue.length
            : isPlainObject(parsedValue)
                ? Object.keys(parsedValue).length
                : 1;

        return {
            topLevelEntries,
            rawBytes: getUtf8Length(rawText)
        };
    }

    function getUtf8Length(value) {
        if (typeof TextEncoder !== "undefined") {
            return new TextEncoder().encode(value).length;
        }

        return unescape(encodeURIComponent(value)).length;
    }

    function isPlainObject(value) {
        return value !== null && typeof value === "object" && !Array.isArray(value);
    }

    function setDefaultHoverMessage(nodeCount, edgeCount) {
        hoverPanel?.classList.remove("graph-panel-error");
        hoverContent.innerHTML = `
            <p class="graph-panel-muted">${nodeCount} nodes and ${edgeCount} relationships are currently visible.</p>
            <p class="graph-panel-empty">Hover a node to see every direct relationship it has with other nodes.</p>
        `;
    }

    function setDefaultPropertiesMessage() {
        if (!nodePropertiesContent) {
            return;
        }

        nodePropertiesContent.innerHTML = "<p class=\"graph-panel-empty\">Hover or select a node to inspect property details.</p>";
    }

    function getNodeId(id) {
        return `node-${id}`;
    }

    function getEdgeId(id) {
        return `edge-${id}`;
    }

    function escapeHtml(value) {
        return String(value)
            .replaceAll("&", "&amp;")
            .replaceAll("<", "&lt;")
            .replaceAll(">", "&gt;")
            .replaceAll('"', "&quot;")
            .replaceAll("'", "&#39;");
    }
})();
