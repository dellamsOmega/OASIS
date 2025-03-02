﻿using System.Collections.Generic;
using System.Threading.Tasks;
using NextGenSoftware.OASIS.Common;
using NextGenSoftware.OASIS.API.Core.Enums;
using NextGenSoftware.OASIS.API.Core.Helpers;
using NextGenSoftware.OASIS.API.Core.Interfaces;
using NextGenSoftware.OASIS.API.Core.Interfaces.Search;
using NextGenSoftware.OASIS.API.Core.Objects.Search;

namespace NextGenSoftware.OASIS.API.Core.Managers
{
    public partial class HolonManager : OASISManager
    {
        public OASISResult<IEnumerable<IHolon>> SearchHolons(string searchTerm, HolonType holonType = HolonType.All, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, bool loadChildrenFromProvider = false, HolonType childHolonType = HolonType.All, int version = 0, ProviderType providerType = ProviderType.Default, bool cache = true)
        {
            OASISResult<IEnumerable<IHolon>> result = new OASISResult<IEnumerable<IHolon>>();
            OASISResult<ISearchResults> searchResults = SearchManager.Instance.Search(new SearchParams()
            {
                SearchGroups = new List<ISearchGroupBase>()
                {
                    new SearchTextGroup()
                    {
                        SearchQuery = searchTerm,
                        SearchAvatars = false,
                        SearchHolons = true,
                        HolonSearchParams = new SearchHolonParams()
                        {
                            SearchAllFields = true
                        }
                    }
                }
            });

            if (searchResults != null && !searchResults.IsError && searchResults.Result != null)
                result.Result = searchResults.Result.SearchResultHolons;

            return result;
        }

        public async Task<OASISResult<IEnumerable<IHolon>>> SearchHolonsAsync(string searchTerm, HolonType holonType = HolonType.All, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, bool loadChildrenFromProvider = false, HolonType childHolonType = HolonType.All, int version = 0, ProviderType providerType = ProviderType.Default, bool cache = true)
        {
            OASISResult<IEnumerable<IHolon>> result = new OASISResult<IEnumerable<IHolon>>();
            OASISResult<ISearchResults> searchResults = await SearchManager.Instance.SearchAsync(new SearchParams() 
            { 
                SearchGroups = new List<ISearchGroupBase>() 
                { 
                    new SearchTextGroup() 
                    { 
                        SearchQuery = searchTerm, 
                        SearchAvatars = false, 
                        SearchHolons = true, 
                        HolonSearchParams = new SearchHolonParams() 
                        { 
                            SearchAllFields = true 
                        } 
                    } 
                } 
            });

            if (searchResults != null && !searchResults.IsError && searchResults.Result != null)
                result.Result = searchResults.Result.SearchResultHolons;

            return result;
        }

        public OASISResult<IEnumerable<T>> SearchHolons<T>(string searchTerm, HolonType holonType = HolonType.All, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, bool loadChildrenFromProvider = false, HolonType childHolonType = HolonType.All, int version = 0, ProviderType providerType = ProviderType.Default, bool cache = true) where T : IHolon
        {
            OASISResult<IEnumerable<T>> result = new OASISResult<IEnumerable<T>>();
            OASISResult<ISearchResults> searchResults = SearchManager.Instance.Search(new SearchParams()
            {
                SearchGroups = new List<ISearchGroupBase>()
                {
                    new SearchTextGroup()
                    {
                        SearchQuery = searchTerm,
                        SearchAvatars = false,
                        SearchHolons = true,
                        HolonSearchParams = new SearchHolonParams()
                        {
                            SearchAllFields = true
                        }
                    }
                }
            });

            if (searchResults != null && !searchResults.IsError && searchResults.Result != null)
                result.Result = Mapper.Convert<IHolon, T>(searchResults.Result.SearchResultHolons);

            return result;
        }

        public async Task<OASISResult<IEnumerable<T>>> SearchHolonsAsync<T>(string searchTerm, HolonType holonType = HolonType.All, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, bool loadChildrenFromProvider = false, HolonType childHolonType = HolonType.All, int version = 0, ProviderType providerType = ProviderType.Default, bool cache = true) where T : IHolon
        {
            OASISResult<IEnumerable<T>> result = new OASISResult<IEnumerable<T>>();
            OASISResult<ISearchResults> searchResults = await SearchManager.Instance.SearchAsync(new SearchParams()
            {
                SearchGroups = new List<ISearchGroupBase>()
                {
                    new SearchTextGroup()
                    {
                        SearchQuery = searchTerm,
                        SearchAvatars = false,
                        SearchHolons = true,
                        HolonSearchParams = new SearchHolonParams()
                        {
                            SearchAllFields = true
                        }
                    }
                }
            });

            if (searchResults != null && !searchResults.IsError && searchResults.Result != null)
                result.Result = Mapper.Convert<IHolon, T>(searchResults.Result.SearchResultHolons);

            return result;
        }
    }
} 