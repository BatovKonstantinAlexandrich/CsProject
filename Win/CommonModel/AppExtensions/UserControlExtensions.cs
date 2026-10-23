using MaterialSkin.Controls;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class UserControlExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="options"></param>
    /// <param name="oncompleted"></param>
    public static void SelectMany(this Control control, string title, IEnumerable<string> options, Action<List<string>> oncompleted)
    {
        List<string> result = new();
        bool completed = false;
        var temp = control.Controls.Count>0? control.Controls[0]: null;
        MaterialCheckedListBox CheckList = control.CreateCheckedListBox(options.ToList());

        var panel = new ControlsFactory().CreateSubmitPanel(title,
            CheckList,
            //new Button() { Text = "OK" },
            (success) =>
            {
                control.LayoutFill(temp);
                result = CheckList.Items.Where(p => p.Checked).Select(p => p.Text).ToList();
                completed = true;
                if (success)
                    oncompleted(result);
            });
        control.LayoutFill(panel);
    }


    public static MaterialCheckedListBox CreateCheckedListBox(this Control control, IEnumerable<string> options)
    {
        var result = new MaterialCheckedListBox();
        foreach (var option in options.OrderByDescending(option => option))
        {
            result.Items.Add(option);
        }
        return result;
    }


    public static MaterialCheckedListBox CreateCheckedListBox(this Control control, Dictionary<string, bool> options, Action<string, bool> onchanged = null)
    {
        try
        {
            var result = new MaterialCheckedListBox();
            foreach (var key in options.Keys.ToList().OrderByDescending(option => option))
            {

                var option = new
                {
                    Key = key,
                    Value = options[key]
                };
                MaterialCheckbox p = null;
                result.Items.Add(p = new MaterialCheckbox()
                {
                    Text = option.Key,
                    Checked = option.Value
                });
                p.CheckedChanged += (s, e) =>
                {

                    if (onchanged is not null)
                    {
                        var checkbox = result.Items.FirstOrDefault(p => p.Text == key);
                        onchanged(key, checkbox.Checked);
                    }
                };

            }
            return result;
        }
        catch (Exception ex)
        {
            control.Error(ex);
            throw;
        }
    }
}

