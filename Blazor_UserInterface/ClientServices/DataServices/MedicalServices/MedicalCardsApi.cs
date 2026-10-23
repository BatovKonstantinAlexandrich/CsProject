using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
 
public class MedicalCardsApi : IEntityFasade<MedicalCard>
{
    private readonly HttpClient _http;

    public MedicalCardsApi(HttpClient http)
    {
        _http = http;
    }

    public int Count(string entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> Create(MedicalCard model)
    {
        throw new NotImplementedException();
    }

    public Task<int> Delete(int id)
    {
        throw new NotImplementedException();
    }

    public Task<MedicalCard> Find(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<MedicalCard[]> List()
    {
        var response =await _http.GetAsync("/MedicalCards/List");
        return await response.Content.ReadFromJsonAsync<MedicalCard[]>();
    }

    public Task<MedicalCard[]> Page(int page, int size)
    {
        throw new NotImplementedException();
    }

    public Task<MedicalCard[]> Page(int page, int size, params string[] sorting)
    {
        throw new NotImplementedException();
    }

    public Task<MedicalCard[]> Search(string query, int page, int size)
    {
        throw new NotImplementedException();
    }

    public Task<MedicalCard[]> Search(string query, int page, int size, params string[] sorting)
    {
        throw new NotImplementedException();
    }

    public Task<int> Update(MedicalCard model)
    {
        throw new NotImplementedException();
    }
}
 