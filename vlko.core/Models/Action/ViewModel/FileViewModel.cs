using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace vlko.core.Models.Action.ViewModel
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
