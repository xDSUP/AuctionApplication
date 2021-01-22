using System;
using System.Collections.Generic;

namespace AutionApp
{
    public partial class State
    {
        public enum StateLot
        {
            /// <summary>Идут торги, открыт</summary>
            OPENED = 1,
            /// <summary>Закрыт</summary>
            CLOSED = 2,
            /// <summary>Ждет начала торгов</summary>
            WAITED = 3,
            /// <summary>Ждет оплаты</summary>
            WAITED_MONEY = 4,
            /// <summary>Ждет отправки</summary>
            WAITED_SENT = 5,
            /// <summary>Доставляется</summary>
            DELIVERED = 6,
            /// <summary>Закончен</summary>
            FINISHED = 7
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
