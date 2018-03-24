using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgileLabs.ContentManager.UEditor
{
    public class UEditorConfig
    {
        public string imageFieldName { get; set; }
        public int imageMaxSize { get; set; }
        public List<string> imageAllowFiles { get; set; }
        public bool imageCompressEnable { get; set; }
        public int imageCompressBorder { get; set; }
        public string imageInsertAlign { get; set; }
        public string imageUrlPrefix { get; set; }
        public string imagePathFormat { get; set; }
    }
}
