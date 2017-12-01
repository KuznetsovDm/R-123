using System.Windows.Controls;
using System.Windows.Xps.Packaging;

namespace R123.Pages
{
    /// <summary>
    /// Логика взаимодействия для XpsDocumentPage.xaml
    /// </summary>
    public partial class XpsDocumentPage : Page
    {
        public XpsDocumentPage(int numberPage)
        {
            InitializeComponent();

            string file = "R-123M";
            XpsDocument xpsDocument = new XpsDocument("../../Files/XSPLearning/" + file + ".xps", System.IO.FileAccess.Read);
            docViewer.Document = xpsDocument.GetFixedDocumentSequence();
            docViewer.GoToPage(numberPage);
        }
        public XpsDocumentPage(string file)
        {
            InitializeComponent();

            XpsDocument xpsDocument = new XpsDocument("../../Files/XSPLearning/" + file + ".xps", System.IO.FileAccess.Read);
            docViewer.Document = xpsDocument.GetFixedDocumentSequence();
        }
    }
}
