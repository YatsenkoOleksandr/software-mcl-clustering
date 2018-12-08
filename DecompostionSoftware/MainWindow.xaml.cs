using ClusteringEntities.Core;
using MCLClustering;
using System.Collections.Generic;
using System.Windows;

namespace DecompostionSoftware
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog();
            openDialog.DefaultExt = "*.jgrokout";
            openDialog.Filter = "JGrok Output (*.jgrokout)|*.jgrokout|Text Documents (*.txt)|*.txt|All Files|*.*";
            if (openDialog.ShowDialog() == true)
            {
                string filename = openDialog.FileName;


                JGrokFileParser parser = new JGrokFileParser(filename);

                ICollection<Entity> entities = parser.Parse();

                MCLEntityClustering mcl = new MCLEntityClustering();

                INode cluster = mcl.Decompose(entities);

                textBoxOutput.Document.Blocks.Clear();
                this.textBoxOutput.AppendText("\n");
                OutputDecompositionResult(cluster, "");
            }
        }

        private void OutputDecompositionResult(INode cluster, string tabs, bool top = true)
        {
            string entity = "";
            if (top)
            {
                entity = string.Format("{0}{1}\n", tabs, cluster.Name);
            }
            else
            {
                entity = string.Format("{0}- {1}: {2}\n", tabs, cluster.ImageType, cluster.Name);
            }

            this.textBoxOutput.AppendText(entity);

            string newTabs = string.Format("{0}   ", tabs);
            foreach(var childNode in cluster.Childs)
            {
                OutputDecompositionResult(childNode, newTabs, false);
            }
        }
    }
}
