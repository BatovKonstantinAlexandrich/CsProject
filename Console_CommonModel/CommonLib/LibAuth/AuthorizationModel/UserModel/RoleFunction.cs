[Label("Функция привязанная к роли")]
public class RoleFunction: IntegratedObject
{
    [Label("Роль пользователей")]
    [InputOrder(2)]
    public int UserRoleId { get; set; }

    [Label("Запрос")]
    [InputOrder(3)]
    public int MessageInput { get; set; }

    [Label("Результат")]
    [InputOrder(4)]
    public int MessageOutput { get; set; }
}