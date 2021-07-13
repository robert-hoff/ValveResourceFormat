using System;
using System.Windows.Forms;

namespace MyGUI.Forms {
    public partial class SearchForm : Form {
        /// <summary>
        /// Gets whatever text was entered by the user in the search textbox.
        /// </summary>
        public string SearchText => findTextBox.Text;

        /// <summary>
        /// Gets whatever options was selected by the user in the search type combobox.
        /// </summary>
        public SearchType SelectedSearchType => ((SearchTypeItem)searchTypeComboBox.SelectedItem).Type;

        public SearchForm() {
            InitializeComponent();

            searchTypeComboBox.ValueMember = "Id";
            searchTypeComboBox.DisplayMember = "Name";
            searchTypeComboBox.Items.Add(new SearchTypeItem("File Name (Partial Match)", SearchType.FileNamePartialMatch));
            searchTypeComboBox.Items.Add(new SearchTypeItem("File Name (Exact Match)", SearchType.FileNameExactMatch));
            searchTypeComboBox.Items.Add(new SearchTypeItem("File Full Path", SearchType.FullPath));
            searchTypeComboBox.Items.Add(new SearchTypeItem("Regex", SearchType.Regex));
            searchTypeComboBox.SelectedIndex = 0;
        }

        private void FindButton_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// On form load, setup the combo box search options and set the textbox as the focused control.
        /// </summary>
        /// <param name="sender">Object which raised event.</param>
        /// <param name="e">Event data.</param>
        private void SearchForm_Load(object sender, EventArgs e) {
            ActiveControl = findTextBox;
        }
    }
}
