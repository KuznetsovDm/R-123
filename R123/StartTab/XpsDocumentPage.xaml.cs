using System.Windows.Controls;
using System.Windows.Xps.Packaging;

namespace R123.StartTab
{
    /// <summary>
    /// Логика взаимодействия для XpsDocumentPage.xaml
    /// </summary>
    public partial class XpsDocumentPage : Page
    {
        public XpsDocumentPage(string file)
        {
            InitializeComponent();

            docViewer.Document = 
                new XpsDocument("../../Files/XSPLearning/" + file + ".xps", System.IO.FileAccess.Read).
                GetFixedDocumentSequence();
        }
    }
}
