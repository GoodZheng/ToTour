using Stateless;
using System.ComponentModel.DataAnnotations;

namespace ToTour.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public ICollection<LineItem> OrderItems { get; set; }

        public OrderStateEnum State { get; set; } //订单目前的状态

        public DateTime CreateDateUTC { get; set; }

        public string TransactionMetadata { get; set; } //第三方支付数据

        StateMachine<OrderStateEnum, OrderStateTriggerEnum> _machine; //私有的状态机变量    <订单状态, 订单状态的触发动作>

        public Order()
        {
            StateMachineInit();
        }

        private void StateMachineInit()
        {
            _machine = new StateMachine<OrderStateEnum, OrderStateTriggerEnum>(
               OrderStateEnum.Pending); //初始化状态Pending初始化状态机

            //_machine = new StateMachine<OrderStateEnum, OrderStateTriggerEnum>
            //    (OrderStateEnum.Pending);

            _machine.Configure(OrderStateEnum.Pending)    //给Pending配置状态转换
                .Permit(OrderStateTriggerEnum.PlaceOrder, OrderStateEnum.Processing)  //如果触发状态PlaceOrder, 则触发状态Processing
                .Permit(OrderStateTriggerEnum.Cancel, OrderStateEnum.Cancelled); //如果触发Cancel, 则触发状态Cancelled

            _machine.Configure(OrderStateEnum.Processing)
                .Permit(OrderStateTriggerEnum.Approve, OrderStateEnum.Completed)
                .Permit(OrderStateTriggerEnum.Reject, OrderStateEnum.Declined);

            _machine.Configure(OrderStateEnum.Declined)
                .Permit(OrderStateTriggerEnum.PlaceOrder, OrderStateEnum.Processing);

            _machine.Configure(OrderStateEnum.Completed)
                .Permit(OrderStateTriggerEnum.Return, OrderStateEnum.Refund);
        }
    }

    public enum OrderStateEnum
    {
        Pending, // 订单已生成
        Processing, // 支付处理中
        Completed, // 交易成功
        Declined, // 交易失败
        Cancelled, // 订单取消
        Refund, // 已退款
    }
    public enum OrderStateTriggerEnum
    {
        PlaceOrder, // 支付
        Approve, // 收款成功
        Reject, // 收款失败
        Cancel, // 取消
        Return // 退货
    }
}
