using System.Collections.Generic;

public class MatrixService
{
     public string[] GetColumnNumbers(int countOfColumns)
     {
        var list = new List<string>();
        for(int i=1; i<= countOfColumns; i++)
        {
            list.Add("№"+i.ToString());
        }
        return list.ToArray();
     }
}