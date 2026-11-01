using System;
namespace MyStore.Models.ViewModel
{
    /// <summary>
    /// ViewModel cho menu cá nhân trên Header (BE#8 - PersonalMenu)
    /// </summary>
    public class PersonalMenuVM
    {
        public bool IsLoggedIn { get; set; }

        public string Username { get; set; }

        public int CartCount { get; set; }
    }
}
