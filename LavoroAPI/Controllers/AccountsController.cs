using System.Net.Http;
using System.Web.Http;
using LavoroAPI.SqlRepository;
using System.Web.Http.Cors;
using LavoroAPI.Models;
using System.Collections.Generic;

namespace LavoroAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AccountsController : LavoroApiController
    {
        AccountsRepository _accountRepository = new AccountsRepository();

        [Route("Accounts/{id}")]
        [HttpGet]
        public HttpResponseMessage GetAccountById(int id)
        {
            Account account = _accountRepository.GetAccountById(id);
            return JsonResponse(account);
        }

        [Route("Accounts/Create")]
        [HttpPost]
        public void CreateAccount([FromBody] Account account)
        {
            _accountRepository.CreateAccount(account);
        }

        [Route("Account/Contacts/{id}")]
        [HttpGet]
        public HttpResponseMessage GetAccountContacts(int id)
        {
            List<Contact> contacts = _accountRepository.GetAccountContacts(id);
            return JsonResponse(contacts);
        }

        [Route("Account/Conversations/{id}")]
        [HttpPost]
        public HttpResponseMessage GetConversation(PhoneLookup request)
        {
            PhoneLookup conversation = _accountRepository.GetConversation(request);
            return JsonResponse(conversation);
        }
    }
}
