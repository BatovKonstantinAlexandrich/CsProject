using MaterialSkin.Controls;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using WinFormsApp.AppViews.SearchModule;

using WinFormsDesignerApp.Console_InputModule.ViewEvents.List;

namespace WinFormsApp.AppComponents.AppControls
{
    /// <summary>
    /// Управление поиском, содержит строку ввода запроса и список подсказок
    /// Переход по списку по нажитию кливиш вниз вверх.
    /// Управление через публичные свойства
    /// </summary>
    public partial class SearchControl : ControlBase, ISearchLine
    {
        private ControlsFactory factory = new();
        private MaterialTextBox2 input = new MaterialTextBox2() { Text = "Поиск" };
        private MaterialListBox list;



        //публичные свойства
        public string SearchQuery
        {
            get => input.Text; set => input.Text = value;
        }
        public ChangingList<string> SearchOptions { get => _searchOptions; set => _searchOptions = value; }

        /*//подсказки при вводе
        public ChangingList<string> SearchOptions
        {
            get
            {
                return _searchOptions;
            }
            set
            {
                list.Items.Clear();
                foreach (var val in _searchOptions = value)
                {
                    if (val.Replace(" ", "").ToLower() != SearchQuery.Replace(" ", "").ToLower())
                    {
                        list.Items.Add(new MaterialSkin.MaterialListBoxItem()
                        {
                            Text = val,
                            Tag = val,
                            SecondaryText = val
                        });
                    }

                }
                Height = 50 + 50 * list.Items.Count();
            }
        }*/

        //утравление панелью с подсказками
        public bool ShowSearchOptions
        {
            get
            {
                return _showSearchOptions;
            }
            set
            {
                _showSearchOptions = value;
                list.Items.Clear();
                if (_showSearchOptions)
                {
                    var _searchOptionsList = _searchOptions.ToList();
                    this.Info(_searchOptionsList.ToJsonOnScreen());
                    foreach (var val in _searchOptionsList)
                    {
                        if (val == SearchQuery)
                            continue;
                        list.Items.Add(new MaterialSkin.MaterialListBoxItem()
                        {
                            Text = val,
                            Tag = val,
                            SecondaryText = val
                        });
                    }
                }
                Height = 50 + 50 * list.Items.Count();
            }
        }

        //внутренние поля
        private bool _showSearchOptions = false;
        private ChangingList<string> _searchOptions = new();
        public Action<string> oninput { get; set; }
        public Action<string> onsearch { get; set; }


        public SearchControl() : this(null, null) { }
        public SearchControl(Action<string> oninput, Action<string> onsearch)
        {
            list = factory.CreateMaterialList(new List<string>() { }, (selected) => { }); ;
            this.LayoutFill(list);
            this.LayoutTop(input);

            this.oninput = (q) =>
            {
                this.Info(SearchOptions.ToJsonOnScreen());
            };
            if (oninput is not null)
                this.oninput += oninput;
            this.onsearch = (q) => { };
            if (onsearch is not null)
                this.onsearch += onsearch;
            this.Height = 75;

            input.Enter += (s, e) =>
            {
                ShowSearchOptions = true;
            };
            input.Leave += (s, e) =>
            {
                ShowSearchOptions = false;
            };
            input.MouseEnter += (s, e) =>
            {
                ShowSearchOptions = true;
            };
            input.MouseLeave += (s, e) =>
            {
                //ShowSearchOptions = false;
            };
            input.KeyDown += (s, e) =>
            {
                System.Console.WriteLine(e.KeyValue);
                switch (e.KeyValue)
                {
                    case 13:
                        DoSearch();
                        break;
                    case 40: if (list.Items.Count() > list.SelectedIndex + 1) list.SelectedIndex++; break;
                    case 38: if (list.SelectedIndex > 0) list.SelectedIndex--; break;
                }
            };
            input.TextChanged += (s, e) =>
            {
                if (this.oninput is not null)
                {
                    this.oninput(input.Text);
                }
            };
            SearchOptions = new();
            SearchOptions.OnChanged += (s, e) =>
            {
                if (ShowSearchOptions)
                {
                    switch (e.Target.Action)
                    {
                        case "added":
                            list.Items.Add(new()
                            {
                                Text = $"{e.Target.Item}"
                            });
                            break;
                        case "removed":
                            list.Items.Remove(list.Items.FirstOrDefault(p => p.Text==$"{e.Target.Item}"));
                            break;
                    }
                }

            };
        }

        private void DoSearch()
        {
            if (list.SelectedIndex >= 0 && list.SelectedIndex < list.Items.Count)
                input.Text = list.Items[list.SelectedIndex].Text;
            if (onsearch is not null)
                onsearch(input.Text == "Поиск" ? "" : input.Text);
        }
    }
}
