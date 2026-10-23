
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;




public class MessageProtocolService
{
    private readonly BusinessDbContext _context;

    public MessageProtocolService( BusinessDbContext context )
    {
        _context = context;
    }

    /*public List<MessageProtocol> GetMessageProtocolsForUser(int userId)
    {
        UserContext  user = _context.UserContexts.Include(u => u.UserGroups).Where(u => u.Id == userId).SingleOrDefault();

        user.Groups = (from g in _context.UserGroups where (from p in user.UserGroups select p.GroupID).Contains(g.Id) select g).ToList();
        var userGroupIDs = (from p in user.Groups select p.Id).ToList();
        var bsfs = (from p in _context.GroupsBusinessFunctions where userGroupIDs.Contains(p.GroupID) select p.BusinessFunctionID).ToList();
        var protocols = (from p in _context.MessageProtocols.Include(p => p.Properties) where p.FromId != null && bsfs.Contains((int)p.FromId) select p).ToList();
        return protocols.ToList();
    }*/


 
    public string ToSql(MessageProtocol model){
        return (new BusinessDatabaseGenerator().CreateTableModel(model).toSql());
    }


    

    /// <summary>
    /// Выбор сведений о протоколе передачи сообщений
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public MessageProtocol GetMessage( int id )
    {
        MessageProtocol protocol = (from p in _context.MessageProtocols where p.Id==id select p).SingleOrDefault();
        protocol.Properties = (from p in _context.MessageProperties.Include(p => p.Attribute) where p.MessageProtocolId == protocol.Id select p).ToList();
        if (protocol == null)
        {
            throw new Exception("Не найден протокол с идентификатором "+id);
        }        
        return protocol;
    }

    public MessageProperty AddProperty(int messageProtocolId, int messageAttributeId)
    {            
        var messageProtocol = _context.MessageProtocols.Find(messageProtocolId);
        if (messageProtocol == null)
        {
            throw new Exception();
        }
        MessageAttribute messageAttribute = _context.MessageAttributes.Find(messageAttributeId);
        if (messageAttribute == null)
        {
            throw new Exception();
        }
        var property = new MessageProperty()
        {
            Label = "Новое свойство",
            Required = false,
            Unique = false,
            Index = false,
            Help = messageAttribute.Description,
            AttributeId = messageAttributeId,
            Name = messageAttribute.Name,
            MessageProtocolId = messageProtocolId
        };
        _context.MessageProperties.Add(property);            
        _context.SaveChanges();
        return property;
    } 

    public MessageAttribute GetMessageAttribute(int messageAttributeId)
    {
        return _context.MessageAttributes.Find(messageAttributeId);
    }

    public void UpdateMessageProperty(MessageProperty messageProperty)
    {
        _context.Update(messageProperty);
        _context.SaveChanges();
    }

    public MessageProperty GetMessageProperty(int messageProtocolId, int messagePropertyId)
    {
        MessageProperty property = _context.MessageProperties.Find(messagePropertyId);
        property.MessageProtocol = _context.MessageProtocols.Find(property.MessageProtocolId);
        return property;
    }
}
