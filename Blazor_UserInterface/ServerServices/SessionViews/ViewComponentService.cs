using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Базовый класс сервисов комопнентов представления.
/// Компоненты представления интегрируются на страницы через эти сервисы.
/// Сервисы отвечаются за связь модели данных с  моделями и событиями представления.
/// </summary>
public abstract class ViewComponentService
{
    private readonly UserModelsService _models;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="models"></param>
    public ViewComponentService(UserModelsService models)
    {
        _models = models;
    }


    public object IntoSession( object viewModel )
    {        
        _models.RegistrateModel(viewModel);
        return viewModel;
    }
}
