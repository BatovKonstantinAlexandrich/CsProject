using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
public class CardService
{
    public global::ExpansionView ForEntity(object record)
    {
        var card = new global::ExpansionView(record.GetType().GetOwnPropertyNames(), record);

        return card;
    }
}