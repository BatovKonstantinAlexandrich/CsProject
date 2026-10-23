using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using Microsoft.EntityFrameworkCore;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

[Label("Личные данные")]
[UniqValues($"{nameof(SurName)},{nameof(FirstName)},{nameof(LastName)}")]
[Index(nameof(SurName),nameof(LastName))]
public class UserPerson: BaseEntity
{
    [NotMapped]
    [NotInputAttribute]
    public string FullName { get => GetFullName(); set => throw new NotImplementedException(); }
    public string GetFullName() => $"{SurName} {FirstName} {LastName}";

    public UserPerson()
    {
        var data = DbContextUserInitializer.GetRandomPersonData();
        SurName = data.Item1;
        FirstName = data.Item2;
        LastName = data.Item3;
        Birthday = data.Item4;
        var tel = data.Item5;
        Tel = tel is null ? null : tel.Replace(" ", "").Replace("-", "");
    }


    public UserPerson(string surName, string firstName, string lastName, DateTime? birthday, string tel)
    {
        SurName = surName;
        FirstName = firstName;
        LastName = lastName;
        Birthday = birthday;
        Tel = tel is null? null: tel.Replace(" ", "").Replace("-", "");
    }


    [InputOrder(1)]
    [Label("Фамилия")]
    [NotNullNotEmpty("Не указана фамилия пользователя")]
    [InputRusText("Записывайте фамилию кирилицей")]
    [Icon("person")]
    public string SurName { get; set; }

    [InputOrder(2)]
    [Label("Имя")]
    [NotNullNotEmpty("Не указано имя пользователя")]
    [InputRusText("Записывайте имя кирилицей")]
    [Icon("person")]
    public string FirstName { get; set; }

    [InputOrder(3)]
    [Label("Отчество")]
    [NotNullNotEmpty("Не указано отчество пользователя")]
    [InputRusText("Записывайте отчество кирилицей")]
    [Icon("person")]
    public string LastName { get; set; }

    [InputOrder(4)]
    [Label("Дата рождения")]
    [InputDate()]
    [NotNullNotEmpty("Не указана дата рождения пользователя")]
    [Icon("person")]
    public DateTime? Birthday { get; set; } 

    [InputOrder(5)]
    [InputPhone("Номер телефона указан неверно")]
    [UniqValue("Этот номер телефона уже зарегистрирован")]
    [Label("Номер телефона")]
    [NotNullNotEmpty("Не указана номер телефона")]
    [Icon("phone")]
    public string Tel { get; set; } 

    [InputOrder(6)]
    [Label("Файл")]
    [InputImage()]
    [Icon("add_a_photo")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public byte[] Data { get; set; }




}
