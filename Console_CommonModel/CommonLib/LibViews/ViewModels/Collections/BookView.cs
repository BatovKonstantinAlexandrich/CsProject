using System.Collections.Generic;

/// <summary>
/// Модель представления постраничного просмотра
/// </summary>
/// <typeparam name="T"></typeparam>
[Label("Постраничная навигация")]
public class BookView: PaginationView
{
    
    [Label("Results")]
    public List<object> Results { get; set; }

    public BookView() {
        //this.Left = null;
        Results = new List<object>() {
           /* PersonNamesProvider.GetRandomPerson()*/
        };
    } 

}
