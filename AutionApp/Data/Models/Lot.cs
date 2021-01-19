using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutionApp
{
    public partial class Lot : IValidatableObject
    {
        public Lot()
        {
            Bids = new HashSet<Bid>();
            States = new HashSet<StatesLots>();
        }

        public int LotId { get; set; }

        [Display(Name="Название")]
        [Required(ErrorMessage = "Не указано название")]
        public string Title { get; set; }

        [Display(Name = "Описание")]
        [DataType(DataType.MultilineText)]
        public string Desc { get; set; }

        [Display(Name = "Фотография")]
        [Column(TypeName = "image")]
        public byte[] Photo { get; set; }

        [Required(ErrorMessage = "Не указана дата начала")]
        [Display(Name = "Дата начала аукциона")]
        public DateTime TimeStart { get; set; }

        [Required(ErrorMessage = "Не указана дата завершения")]
        [Display(Name = "Дата завершения аукциона")]
        public DateTime TimeEnd { get; set; }

        [Display(Name = "Начальная цена")]
        [Range(0, int.MaxValue)]
        public decimal StartPrice { get; set; }

        [Display(Name = "Минимальный шаг для ставок")]
        [Range(1, int.MaxValue)]
        public decimal Step { get; set; }

        [Required(ErrorMessage = "Не указана категория")]
        [Display(Name = "Категория лота")]
        public int CategoryId { get; set; }

        [Column("UserId")]
        [Display(Name = "Создатель лота")]
        public string UserId { get; set; }

        public virtual Category Category { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Bid> Bids { get; set; }
        public virtual ICollection<StatesLots> States { get; set; }
        public virtual ICollection<Sell> Sells { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(TimeStart < DateTime.Now)
                yield return new ValidationResult($"Дата начала аукциона не может быть раньше текущего времени", new[] { nameof(TimeStart)});
            if (TimeStart > TimeEnd)
                yield return new ValidationResult($"Дата завершения аукциона не может быть раньше начала", new[] { nameof(TimeStart), nameof(TimeEnd) });
            //TODO: еще проверки
        }
    }
}
