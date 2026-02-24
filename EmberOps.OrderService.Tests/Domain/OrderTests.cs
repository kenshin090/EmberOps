using EmberOps.OrderService.Domain.Common;
using EmberOps.OrderService.Domain.Order;
using EmberOps.OrderService.Domain.Order.Enums;
using FluentAssertions;

namespace EmberOps.OrderService.Tests.Domain
{
    public class OrderTests
    {
        [Fact]
        public void Create_Should_Set_Initial_State()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            // Act
            var order = new Order(orderId, DateTime.UtcNow);

            // Assert
            order.Should().NotBeNull();
            order.Id.Should().Be(orderId);
            order.Status.Should().Be(OrderStatus.Draft);
        }

        [Fact]
        public void AddItem_Should_Increase_Total()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(orderId, DateTime.UtcNow);
            order.TotalAmount.Should().Be(0);

            // Act
            order.AddItem("abc123", "this is an test", 3, 5);

            // Assert
            order.TotalAmount.Should().Be(15);
            order.Items.Should().HaveCount(1);
        }

        [Fact]
        public void RemoveItem_Should_Decrease_Total()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(orderId, DateTime.UtcNow);
            order.TotalAmount.Should().Be(0);
            order.AddItem("abc123", "this is an test", 3, 5);
            order.AddItem("abc124", "this is an test 2", 4, 3);
            order.TotalAmount.Should().Be(27);
            order.Items.Should().HaveCount(2);

            //Act
            order.RemoveItem(order.Items.First().Id);

            // Assert
            order.TotalAmount.Should().Be(12);
            order.Items.Should().HaveCount(1);
        }

        [Fact]
        public void AddItem_With_Zero_Quantity_Should_Throw()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(orderId, DateTime.UtcNow);

            //Act
            Action act = () => order.AddItem("abc123", "this is an test", 3, 0);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void ChangeItemQuantity_With_Zero_Quantity_Should_Throw()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(orderId, DateTime.UtcNow);

            order.AddItem("abc123", "this is an test", 3, 5);

            //Act
            Action act = () => order.ChangeItemQuantity(order.Items.First().Id, 0);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void ChangeItemQuantity_With_Value_Quantity_Should_Increase()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(orderId, DateTime.UtcNow);

            order.AddItem("abc123", "this is an test", 3, 5);

            var originalQuantity = order.Items.First().Quantity; ;

            //Act
            order.ChangeItemQuantity(order.Items.First().Id, 6);

            // Assert
            order.Items.First().Quantity.Should().BeGreaterThan(originalQuantity);
        }

        [Fact]
        public void ChangeItemQuantity_With_Value_Quantity_Should_Decrease()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(orderId, DateTime.UtcNow);

            order.AddItem("abc123", "this is an test", 3, 5);

            var originalQuantity = order.Items.First().Quantity; ;

            //Act
            order.ChangeItemQuantity(order.Items.First().Id, 3);

            // Assert
            order.Items.First().Quantity.Should().BeLessThanOrEqualTo(originalQuantity);
        }


        [Fact]
        public void SubmitOrder_With_Zero_Products_Should_Throw()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(orderId, DateTime.UtcNow);
                        

            //Act
            Action act = () => order.Submit(DateTime.UtcNow);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void SubmitOrder_With_Status_Draff_Should_Change_Status_To_Submited()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(orderId, DateTime.UtcNow);
            order.AddItem("abc123", "this is an test", 3, 5);
            order.Status.Should().Be(OrderStatus.Draft);

            //Act
            order.Submit(DateTime.UtcNow);

            // Assert
            order.Status.Should().Be(OrderStatus.Submitted);
        }

        [Fact]
        public void MarksAsPayOrder_With_Status_Draff_Should_Throw()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(orderId, DateTime.UtcNow);

            //Act
            Action act = () => order.MarkAsPaid(DateTime.UtcNow);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void MarksAsPayOrder_With_Status_Submited_Should_Change_Status_To_Paid()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(orderId, DateTime.UtcNow);
            order.AddItem("abc123", "this is an test", 3, 5);
            order.Status.Should().Be(OrderStatus.Draft);
            order.Submit(DateTime.UtcNow);
            order.Status.Should().Be(OrderStatus.Submitted);

            //Act
            order.MarkAsPaid(DateTime.UtcNow);

            // Assert
            order.Status.Should().Be(OrderStatus.Paid);
        }

        [Fact]
        public void CancelOrder_With_Status_Paid_Should_Throw()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(orderId, DateTime.UtcNow);
            order.AddItem("abc123", "this is an test", 3, 5);
            order.Status.Should().Be(OrderStatus.Draft);
            order.Submit(DateTime.UtcNow);
            order.Status.Should().Be(OrderStatus.Submitted);
            order.MarkAsPaid(DateTime.UtcNow);

            //Act
            Action act = () => order.Cancel(DateTime.UtcNow,"Cancel by test");

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void CancelOrder_With_Status_Different_To_Paid_Should_ChangeStatusToCancelled()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(orderId, DateTime.UtcNow);
            order.AddItem("abc123", "this is an test", 3, 5);
            order.Status.Should().Be(OrderStatus.Draft);
            order.Submit(DateTime.UtcNow);
            order.Status.Should().Be(OrderStatus.Submitted);            

            //Act
            order.Cancel(DateTime.UtcNow, "Cancel by test");

            // Assert
            order.Status.Should().Be(OrderStatus.Cancelled);
        }



    }
}
