using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ghostlight.Shared;

namespace ghostlight.Client.Services
{
    //https://chrissainty.com/3-ways-to-communicate-between-components-in-blazor/
    public class AppState
    {
        public event Action OnChange;

        private API _api;
        private ILocalStorageService _localStorage;
        private AuthenticationStateProvider _authenticationStateProvider;

        public AppState(API api, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider)
        {
            _api = api;
            _localStorage = localStorage;
            AuthorizedFolders = new List<FolderAuthorization>();

            _authenticationStateProvider = authenticationStateProvider;
        }

        async public Task<bool> UpdateAppState(string selectedFolderId = null)
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity.IsAuthenticated)
            {
                Console.WriteLine("AppState.Start IsAuthed");
                AuthorizedFolders = await _api.GetOrgnaizationAuthorization();

                if (AuthorizedFolders.Count > 0)
                {
                    Console.WriteLine($"AuthedFolders: {AuthorizedFolders.Count}");
                    if (selectedFolderId == null)
                    {
                        selectedFolderId = await _localStorage.GetItemAsync<string>("FolderId");
                    }

                    if (selectedFolderId != null)
                    {
                        var selectedFolder = AuthorizedFolders.FirstOrDefault(g => g.Id == selectedFolderId);
                        if (selectedFolder != null)
                        {
                            CurrentFolderName = selectedFolder.Name;
                            CurrentFolderId = selectedFolder.Id;

                            await _localStorage.SetItemAsync("FolderId", selectedFolderId);
                        }
                    }
                    else
                    {
                        //No Folder. Clear it all.
                        CurrentFolderName = null;
                        CurrentFolderId = null;

                        await _localStorage.RemoveItemAsync("FolderId");
                    }
                }
                else
                {
                    AuthorizedFolders = new List<FolderAuthorization>();
                    CurrentFolderId = default(string);
                    CurrentFolderName = default(string);
                }
                return true;
            }
            Console.WriteLine("NOT AUTHED YET");

            return false;
        }


        private void NotifyStateChanged() => OnChange?.Invoke();

        private string _currentFolderId;
        public string CurrentFolderId
        {
            get
            {
                return _currentFolderId;
            }
            set
            {
                _currentFolderId = value;
                NotifyStateChanged();
            }
        }

        private string _currentFolderName;
        public string CurrentFolderName
        {
            get
            {
                return _currentFolderName;
            }
            set
            {
                _currentFolderName = value;
                NotifyStateChanged();
            }
        }

        private List<FolderAuthorization> _authorizedFolders;
        public List<FolderAuthorization> AuthorizedFolders
        {
            get
            {
                return _authorizedFolders;
            }
            set
            {
                _authorizedFolders = value;
                NotifyStateChanged();
            }
        }

    }
}
