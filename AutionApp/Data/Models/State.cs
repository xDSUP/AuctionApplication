using System;
using System.Collections.Generic;

namespace AutionApp
{
    public partial class State
    {
        public enum StateLot
        {
            /// <summary>Идут торги, открыт</summary>
            OPENED,
            /// <summary>Ждет начала торгов</summary>
            WAITED,
            /// <summary>Закрыт</summary>
            CLOSED,
            /// <summary>Ждет оплаты</summary>
            WAITED_MONEY,
            /// <summary>Ждет отправки</summary>
            WAITED_SENT,
            /// <summary>Доставляется</summary>
            DELIVERED,
            /// <summary>Закончен</summary>
            FINISHED
        }

        public static string getText(StateLot state)
        {
            switch (state)
            {
                case StateLot.OPENED: return "Открыт";
                case StateLot.CLOSED: return "Закрыт";
                case StateLot.WAITED: return "Ждёт начала";
                case StateLot.WAITED_MONEY: return "Ждет оплаты";
                case StateLot.WAITED_SENT: return "Ждет отправки";
                case StateLot.DELIVERED: return "Доставляется";
                case StateLot.FINISHED: return "Завершен";
                    // сюда зайти не должен
                default: throw new NotImplementedException();
            }
        }

        public State()
        {
            StatesLots = new HashSet<StatesLots>();
        }

        public int StateId { get; set; }
        public string Title { get; set; }

        public virtual ICollection<StatesLots> StatesLots { get; set; }
    }
}
