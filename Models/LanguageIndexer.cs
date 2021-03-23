using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Data;
using System.Xml.Linq;

namespace Sudoku.Models
{
    class LanguageIndexer : INotifyPropertyChanged
    {
        Dictionary<string, List<string>> language;
        int selectedLanguageIndex;

        public event PropertyChangedEventHandler PropertyChanged;

        public string this[string index] 
        {
            get
            {
                if (!language.ContainsKey(index) || selectedLanguageIndex >= language[index].Count) return index;
                return language[index][selectedLanguageIndex];
            }
            private set { }
        }

        public LanguageIndexer(string path)
        {
            selectedLanguageIndex = 0;

            //https://stackoverflow.com/questions/12554186/how-to-serialize-deserialize-to-dictionaryint-string-from-custom-xml-not-us
            XElement element;
            using (Stream reader = new FileStream(path, FileMode.Open))
            {
                element = XElement.Load(reader);
            }
            language = element.Descendants("languages").ToDictionary(x => (string)x.Attribute("id"), x => (List<string>)x.Descendants("language").Select(d => d.Value).ToList());
        }

        /// <summary>
        /// Changes active language to next language.
        /// </summary>
        public void ChangeLanguage()
        {
            selectedLanguageIndex++;
            if (selectedLanguageIndex >= language["LANGUAGES"].Count) selectedLanguageIndex = 0;
            OnPropertyChanged(Binding.IndexerName);
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
