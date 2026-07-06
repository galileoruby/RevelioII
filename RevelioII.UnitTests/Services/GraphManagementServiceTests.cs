using Moq;
using RevelioII.Models;
using RevelioII.Repositories;
using RevelioII.Services;

namespace RevelioII.UnitTests.Services
{
    public class GraphManagementServiceTests
    {
        [Fact]
        public async Task GetGraphViewAsync_WhenGraphContainsNodesAndRelationships_ReturnsMappedDto()
        {
            // Arrange
            var nodes = new[]
            {
                new Node { Id = 1, Label = "Alpha", Type = "System", Properties = "{\"color\":\"red\"}" },
                new Node { Id = 2, Label = "Beta", Type = "User", Properties = null },
            };

            var relationships = new[]
            {
                new Relationship
                {
                    Id = 10,
                    SourceNodeId = 1,
                    TargetNodeId = 2,
                    RelationType = "links-to",
                    Properties = "{\"weight\":1}",
                    SourceNode = nodes[0],
                    TargetNode = nodes[1],
                },
                new Relationship
                {
                    Id = 11,
                    SourceNodeId = 3,
                    TargetNodeId = 4,
                    RelationType = "depends-on",
                    Properties = null,
                    SourceNode = null,
                    TargetNode = null,
                },
            };

            var repositoryMock = new Mock<IGraphRepository>();
            repositoryMock
                .Setup(repository => repository.GetGraphAsync())
                .ReturnsAsync((nodes, relationships));

            var service = new GraphManagementService(repositoryMock.Object);

            // Act
            var result = await service.GetGraphViewAsync();

            // Assert
            Assert.Equal(2, result.Nodes.Count);
            Assert.Equal(2, result.Edges.Count);

            var firstNode = result.Nodes.Single(node => node.Id == 1);
            Assert.Equal("Alpha", firstNode.Label);
            Assert.Equal("System", firstNode.Type);
            Assert.Equal("{\"color\":\"red\"}", firstNode.Properties);

            var firstEdge = result.Edges.Single(edge => edge.Id == 10);
            Assert.Equal(1, firstEdge.SourceNodeId);
            Assert.Equal(2, firstEdge.TargetNodeId);
            Assert.Equal("links-to", firstEdge.RelationType);
            Assert.Equal("{\"weight\":1}", firstEdge.Properties);
            Assert.Equal("Alpha", firstEdge.SourceLabel);
            Assert.Equal("Beta", firstEdge.TargetLabel);

            var secondEdge = result.Edges.Single(edge => edge.Id == 11);
            Assert.Equal("Node 3", secondEdge.SourceLabel);
            Assert.Equal("Node 4", secondEdge.TargetLabel);

            repositoryMock.Verify(repository => repository.GetGraphAsync(), Times.Once);
        }

        [Fact]
        public async Task Constructor_WithRepository_UsesRepositoryForSubsequentCalls()
        {
            // Arrange
            var expectedNode = new Node { Id = 42, Label = "Created", Type = "Process", Properties = null };
            var repositoryMock = new Mock<IGraphRepository>();
            repositoryMock
                .Setup(repository => repository.GetNodeByIdAsync(42))
                .ReturnsAsync(expectedNode);

            // Act
            var service = new GraphManagementService(repositoryMock.Object);
            var result = await service.GetNodeAsync(42);

            // Assert
            Assert.Same(expectedNode, result);
            repositoryMock.Verify(repository => repository.GetNodeByIdAsync(42), Times.Once);
        }

        [Fact]
        public async Task GetAllNodesAsync_WhenRepositoryReturnsNodes_ReturnsNodes()
        {
            // Arrange
            var expectedNodes = new[]
            {
                new Node { Id = 1, Label = "Alpha", Type = "System", Properties = null },
                new Node { Id = 2, Label = "Beta", Type = "User", Properties = "{}" },
            };

            var repositoryMock = new Mock<IGraphRepository>();
            repositoryMock
                .Setup(repository => repository.GetNodesAsync())
                .ReturnsAsync(expectedNodes);

            var service = new GraphManagementService(repositoryMock.Object);

            // Act
            var result = await service.GetAllNodesAsync();

            // Assert
            Assert.Same(expectedNodes, result);
            repositoryMock.Verify(repository => repository.GetNodesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetNodeAsync_WhenRepositoryReturnsNull_ReturnsNull()
        {
            // Arrange
            var repositoryMock = new Mock<IGraphRepository>();
            repositoryMock
                .Setup(repository => repository.GetNodeByIdAsync(99))
                .ReturnsAsync((Node?)null);

            var service = new GraphManagementService(repositoryMock.Object);

            // Act
            var result = await service.GetNodeAsync(99);

            // Assert
            Assert.Null(result);
            repositoryMock.Verify(repository => repository.GetNodeByIdAsync(99), Times.Once);
        }

        [Fact]
        public async Task CreateNodeAsync_WhenNodeIsProvided_ReturnsCreatedNode()
        {
            // Arrange
            var node = new Node { Id = 0, Label = "Gamma", Type = "Device", Properties = "{\"active\":true}" };
            var createdNode = new Node { Id = 7, Label = "Gamma", Type = "Device", Properties = "{\"active\":true}" };

            var repositoryMock = new Mock<IGraphRepository>();
            repositoryMock
                .Setup(repository => repository.AddNodeAsync(node))
                .ReturnsAsync(createdNode);

            var service = new GraphManagementService(repositoryMock.Object);

            // Act
            var result = await service.CreateNodeAsync(node);

            // Assert
            Assert.Same(createdNode, result);
            repositoryMock.Verify(repository => repository.AddNodeAsync(node), Times.Once);
        }
        [Fact]
        public async Task CreateRelationshipAsync_WhenSourceAndTargetNodesExist_ReturnsCreatedRelationship()
        {
            // Arrange
            var relationship = new Relationship
            {
                Id = 0,
                SourceNodeId = 1,
                TargetNodeId = 2,
                RelationType = "connects-to",
                Properties = "{\"weight\":5}",
            };
            var sourceNode = new Node { Id = 1, Label = "Source", Type = "System", Properties = null };
            var targetNode = new Node { Id = 2, Label = "Target", Type = "System", Properties = null };
            var createdRelationship = new Relationship
            {
                Id = 15,
                SourceNodeId = 1,
                TargetNodeId = 2,
                RelationType = "connects-to",
                Properties = "{\"weight\":5}",
            };

            var repositoryMock = new Mock<IGraphRepository>();
            repositoryMock
                .Setup(repository => repository.GetNodeByIdAsync(1))
                .ReturnsAsync(sourceNode);
            repositoryMock
                .Setup(repository => repository.GetNodeByIdAsync(2))
                .ReturnsAsync(targetNode);
            repositoryMock
                .Setup(repository => repository.AddRelationshipAsync(relationship))
                .ReturnsAsync(createdRelationship);

            var service = new GraphManagementService(repositoryMock.Object);

            // Act
            var result = await service.CreateRelationshipAsync(relationship);

            // Assert
            Assert.Same(createdRelationship, result);
            repositoryMock.Verify(repository => repository.GetNodeByIdAsync(1), Times.Once);
            repositoryMock.Verify(repository => repository.GetNodeByIdAsync(2), Times.Once);
            repositoryMock.Verify(repository => repository.AddRelationshipAsync(relationship), Times.Once);
        }

        [Fact]
        public async Task UpdateNodeAsync_WhenNodeIsProvided_DelegatesToRepository()
        {
            // Arrange
            var node = new Node { Id = 3, Label = "Gamma", Type = "Service", Properties = "{}" };
            var updateTask = Task.CompletedTask;
            var repositoryMock = new Mock<IGraphRepository>();
            repositoryMock
                .Setup(repository => repository.UpdateNodeAsync(node))
                .Returns(updateTask);

            var service = new GraphManagementService(repositoryMock.Object);

            // Act
            var result = service.UpdateNodeAsync(node);
            await result;

            // Assert
            Assert.Same(updateTask, result);
            repositoryMock.Verify(repository => repository.UpdateNodeAsync(node), Times.Once);
        }

        [Fact]
        public async Task DeleteNodeAsync_WhenIdIsProvided_DelegatesToRepository()
        {
            // Arrange
            const int nodeId = 8;
            var deleteTask = Task.CompletedTask;
            var repositoryMock = new Mock<IGraphRepository>();
            repositoryMock
                .Setup(repository => repository.DeleteNodeAsync(nodeId))
                .Returns(deleteTask);

            var service = new GraphManagementService(repositoryMock.Object);

            // Act
            var result = service.DeleteNodeAsync(nodeId);
            await result;

            // Assert
            Assert.Same(deleteTask, result);
            repositoryMock.Verify(repository => repository.DeleteNodeAsync(nodeId), Times.Once);
        }

        [Fact]
        public async Task GetAllRelationshipsAsync_WhenRepositoryReturnsRelationships_ReturnsRelationships()
        {
            // Arrange
            var expectedRelationships = new[]
            {
                new Relationship { Id = 1, SourceNodeId = 2, TargetNodeId = 3, RelationType = "owns", Properties = null },
                new Relationship { Id = 2, SourceNodeId = 3, TargetNodeId = 4, RelationType = "depends-on", Properties = "{}" },
            };
            var repositoryMock = new Mock<IGraphRepository>();
            repositoryMock
                .Setup(repository => repository.GetRelationshipsAsync())
                .ReturnsAsync(expectedRelationships);

            var service = new GraphManagementService(repositoryMock.Object);

            // Act
            var result = await service.GetAllRelationshipsAsync();

            // Assert
            Assert.Same(expectedRelationships, result);
            repositoryMock.Verify(repository => repository.GetRelationshipsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetRelationshipAsync_WhenRepositoryReturnsRelationship_ReturnsRelationship()
        {
            // Arrange
            var expectedRelationship = new Relationship
            {
                Id = 12,
                SourceNodeId = 5,
                TargetNodeId = 7,
                RelationType = "contains",
                Properties = "{\"enabled\":true}",
            };
            var repositoryMock = new Mock<IGraphRepository>();
            repositoryMock
                .Setup(repository => repository.GetRelationshipByIdAsync(12))
                .ReturnsAsync(expectedRelationship);

            var service = new GraphManagementService(repositoryMock.Object);

            // Act
            var result = await service.GetRelationshipAsync(12);

            // Assert
            Assert.Same(expectedRelationship, result);
            repositoryMock.Verify(repository => repository.GetRelationshipByIdAsync(12), Times.Once);
        }

        [Fact]
        public async Task GetRelationshipAsync_WhenRepositoryReturnsNull_ReturnsNull()
        {
            // Arrange
            var repositoryMock = new Mock<IGraphRepository>();
            repositoryMock
                .Setup(repository => repository.GetRelationshipByIdAsync(77))
                .ReturnsAsync((Relationship?)null);

            var service = new GraphManagementService(repositoryMock.Object);

            // Act
            var result = await service.GetRelationshipAsync(77);

            // Assert
            Assert.Null(result);
            repositoryMock.Verify(repository => repository.GetRelationshipByIdAsync(77), Times.Once);
        }

        [Fact]
        public async Task CreateRelationshipAsync_WhenSourceNodeDoesNotExist_ThrowsArgumentException()
        {
            // Arrange
            var relationship = new Relationship
            {
                SourceNodeId = 41,
                TargetNodeId = 42,
                RelationType = "links-to",
                Properties = null,
            };
            var targetNode = new Node { Id = 42, Label = "Target", Type = "System", Properties = null };
            var repositoryMock = new Mock<IGraphRepository>();
            repositoryMock
                .Setup(repository => repository.GetNodeByIdAsync(41))
                .ReturnsAsync((Node?)null);
            repositoryMock
                .Setup(repository => repository.GetNodeByIdAsync(42))
                .ReturnsAsync(targetNode);

            var service = new GraphManagementService(repositoryMock.Object);

            // Act
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CreateRelationshipAsync(relationship));

            // Assert
            Assert.Equal("Source Node with Id 41 does not exist.", exception.Message);
            repositoryMock.Verify(repository => repository.GetNodeByIdAsync(41), Times.Once);
            repositoryMock.Verify(repository => repository.GetNodeByIdAsync(42), Times.Once);
            repositoryMock.Verify(repository => repository.AddRelationshipAsync(It.IsAny<Relationship>()), Times.Never);
        }

        [Fact]
        public async Task CreateRelationshipAsync_WhenTargetNodeDoesNotExist_ThrowsArgumentException()
        {
            // Arrange
            var relationship = new Relationship
            {
                SourceNodeId = 51,
                TargetNodeId = 52,
                RelationType = "links-to",
                Properties = null,
            };
            var sourceNode = new Node { Id = 51, Label = "Source", Type = "System", Properties = null };
            var repositoryMock = new Mock<IGraphRepository>();
            repositoryMock
                .Setup(repository => repository.GetNodeByIdAsync(51))
                .ReturnsAsync(sourceNode);
            repositoryMock
                .Setup(repository => repository.GetNodeByIdAsync(52))
                .ReturnsAsync((Node?)null);

            var service = new GraphManagementService(repositoryMock.Object);

            // Act
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CreateRelationshipAsync(relationship));

            // Assert
            Assert.Equal("Target Node with Id 52 does not exist.", exception.Message);
            repositoryMock.Verify(repository => repository.GetNodeByIdAsync(51), Times.Once);
            repositoryMock.Verify(repository => repository.GetNodeByIdAsync(52), Times.Once);
            repositoryMock.Verify(repository => repository.AddRelationshipAsync(It.IsAny<Relationship>()), Times.Never);
        }





        [Fact]
        public async Task UpdateRelationshipAsync_WhenRelationshipIsProvided_DelegatesToRepository()
        {
            // Arrange
            var relationship = new Relationship
            {
                Id = 9,
                SourceNodeId = 1,
                TargetNodeId = 2,
                RelationType = "depends-on",
                Properties = "{\"priority\":1}",
            };
            var updateTask = Task.CompletedTask;
            var repositoryMock = new Mock<IGraphRepository>();
            repositoryMock
                .Setup(repository => repository.UpdateRelationshipAsync(relationship))
                .Returns(updateTask);

            var service = new GraphManagementService(repositoryMock.Object);

            // Act
            var result = service.UpdateRelationshipAsync(relationship);
            await result;

            // Assert
            Assert.Same(updateTask, result);
            repositoryMock.Verify(repository => repository.UpdateRelationshipAsync(relationship), Times.Once);
        }

        [Fact]
        public async Task DeleteRelationshipAsync_WhenIdIsProvided_DelegatesToRepository()
        {
            // Arrange
            const int relationshipId = 13;
            var deleteTask = Task.CompletedTask;
            var repositoryMock = new Mock<IGraphRepository>();
            repositoryMock
                .Setup(repository => repository.DeleteRelationshipAsync(relationshipId))
                .Returns(deleteTask);

            var service = new GraphManagementService(repositoryMock.Object);

            // Act
            var result = service.DeleteRelationshipAsync(relationshipId);
            await result;

            // Assert
            Assert.Same(deleteTask, result);
            repositoryMock.Verify(repository => repository.DeleteRelationshipAsync(relationshipId), Times.Once);
        }


    }
}
