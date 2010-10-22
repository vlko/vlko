using System.ComponentModel.DataAnnotations;

namespace vlko.model.Action.ViewModel
{
    public class FileViewModel
    {
        [Key]
        [Editable(false)]
        public string Ident { get; set; }

        [Display(ResourceType = typeof(ModelResources), Name = "Url")]
        [Editable(false)]
        public string Url { get; set; }

        [Display(ResourceType = typeof(ModelResources), Name = "Size")]
        [Editable(false)]
        public long Size { get; set; }
    }
}
