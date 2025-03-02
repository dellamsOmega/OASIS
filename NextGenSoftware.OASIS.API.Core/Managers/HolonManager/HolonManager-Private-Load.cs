﻿using System;
using System.Threading.Tasks;
using NextGenSoftware.Logging;
using NextGenSoftware.OASIS.Common;
using NextGenSoftware.OASIS.API.DNA;
using NextGenSoftware.OASIS.API.Core.Enums;
using NextGenSoftware.OASIS.API.Core.Helpers;
using NextGenSoftware.OASIS.API.Core.Interfaces;

namespace NextGenSoftware.OASIS.API.Core.Managers
{
    public partial class HolonManager : OASISManager
    {
        private OASISResult<IHolon> LoadHolonForProviderType(Guid id, ProviderType providerType, OASISResult<IHolon> result, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, bool loadChildrenFromProvider = false, int version = 0)
        {
            string errorMessage = string.Concat("An error occured in HolonManager.LoadHolonForProviderType attempting to load the holon for id ", id, " using the ", Enum.GetName(providerType), " provider. Reason: ");

            try
            {
                OASISResult<IOASISStorageProvider> providerResult = ProviderManager.Instance.SetAndActivateCurrentStorageProvider(providerType);

                if (!providerResult.IsError && providerResult.Result != null)
                {
                    OASISResult<IHolon> holonResult = Task.Run(() => providerResult.Result.LoadHolon(id, loadChildren, recursive, maxChildDepth, continueOnError, loadChildrenFromProvider, version)).WaitAsync(TimeSpan.FromSeconds(OASISDNA.OASIS.StorageProviders.ProviderMethodCallTimeOutSeconds)).Result;

                    if (holonResult != null)
                    {
                        if (holonResult.IsError || holonResult.Result == null)
                        {
                            if (string.IsNullOrEmpty(holonResult.Message))
                                holonResult.Message = "No Holon Found.";

                            OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from provider calling LoadHolon: ", holonResult.Message), result.DetailedMessage);
                        }
                        else
                        {
                            result.IsLoaded = true;
                            result.LoadedCount = 1;
                            result.Result = holonResult.Result;
                        }
                    }
                    else
                        OASISErrorHandling.HandleWarning(ref result, $"{errorMessage}Result returned from provider calling LoadHolon is null.");
                }
                else
                    OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from ProviderManager.Instance.SetAndActivateCurrentStorageProvider: ", providerResult.Message), providerResult.DetailedMessage);
            }
            catch (TimeoutException)
            {
                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Timeout occured from provider calling LoadHolon."));
            }
            catch (Exception ex)
            {
                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, ex.Message));
            }

            return result;
        }

        private OASISResult<T> LoadHolonForProviderType<T>(Guid id, ProviderType providerType, OASISResult<T> result, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, bool loadChildrenFromProvider = false, int version = 0) where T : IHolon, new()
        {
            string errorMessage = string.Concat("An error occured in HolonManager.LoadHolonForProviderType<T> attempting to load the holon for id ", id, " using the ", Enum.GetName(providerType), " provider. Reason: ");

            try
            {
                OASISResult<IOASISStorageProvider> providerResult = ProviderManager.Instance.SetAndActivateCurrentStorageProvider(providerType);

                if (!providerResult.IsError && providerResult.Result != null)
                {
                    OASISResult<IHolon> holoResult = Task.Run(() => providerResult.Result.LoadHolon(id, loadChildren, recursive, maxChildDepth, continueOnError, loadChildrenFromProvider, version)).WaitAsync(TimeSpan.FromSeconds(OASISDNA.OASIS.StorageProviders.ProviderMethodCallTimeOutSeconds)).Result;

                    if (holoResult != null)
                    {
                        if (holoResult.IsError || holoResult.Result == null)
                        {
                            if (string.IsNullOrEmpty(holoResult.Message))
                                holoResult.Message = "No Holon Found.";

                            OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from provider calling LoadHolon: ", holoResult.Message), result.DetailedMessage);
                        }
                        else
                        {
                            result.IsLoaded = true;
                            result.LoadedCount = 1;
                            result.Result = Mapper.MapBaseHolonPropertiesAndCreateT2IfNull<IHolon, T>(holoResult.Result);
                        }
                    }
                    else
                        OASISErrorHandling.HandleWarning(ref result, $"{errorMessage}Result returned from provider calling LoadHolon is null.");
                }
                else
                    OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from ProviderManager.Instance.SetAndActivateCurrentStorageProvider: ", providerResult.Message), providerResult.DetailedMessage);
            }
            catch (TimeoutException)
            {
                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Timeout occured from provider calling LoadHolon."));
            }
            catch (Exception ex)
            {
                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, ex.Message));
            }

            return result;
        }

        //TODO: Finish Upgrading rest of HolonManager to work with this improved code (from AvatarManager):
        private async Task<OASISResult<IHolon>> LoadHolonForProviderTypeAsync(Guid id, ProviderType providerType, OASISResult<IHolon> result, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, bool loadChildrenFromProvider = false, int version = 0)
        {
            string errorMessage = string.Concat("An error occured in HolonManager.LoadHolonForProviderTypeAsync attempting to load the holon for id ", id, " using the ", Enum.GetName(providerType), " provider. Reason: ");

            try
            {
                OASISResult<IOASISStorageProvider> providerResult = await ProviderManager.Instance.SetAndActivateCurrentStorageProviderAsync(providerType);

                if (!providerResult.IsError && providerResult.Result != null)
                {
                    var task = providerResult.Result.LoadHolonAsync(id, loadChildren, recursive, maxChildDepth, continueOnError, loadChildrenFromProvider, version);

                    if (await Task.WhenAny(task, Task.Delay(OASISDNA.OASIS.StorageProviders.ProviderMethodCallTimeOutSeconds * 1000)) == task)
                    {
                        if (task != null && task.Result != null)
                        {
                            if (task.Result.IsError || task.Result.Result == null)
                            {
                                if (string.IsNullOrEmpty(task.Result.Message))
                                    task.Result.Message = "No Holon Found.";

                                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from provider calling LoadHolonAsync: ", task.Result.Message), task.Result.DetailedMessage);
                            }
                            else
                            {
                                result.IsLoaded = true;
                                result.LoadedCount = 1;
                                result.Result = task.Result.Result;
                            }
                        }
                        else
                            OASISErrorHandling.HandleWarning(ref result, $"{errorMessage}Result returned from provider calling LoadHolonAsync is null.");
                    }
                    else
                        OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Timeout occured from provider calling LoadHolonAsync."));
                }
                else
                    OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from ProviderManager.Instance.SetAndActivateCurrentStorageProviderAsync: ", providerResult.Message), providerResult.DetailedMessage);
            }
            catch (Exception ex)
            {
                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, ex.Message));
            }

            return result;
        }

        private async Task<OASISResult<T>> LoadHolonForProviderTypeAsync<T>(Guid id, ProviderType providerType, OASISResult<T> result, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, bool loadChildrenFromProvider = false, int version = 0) where T : IHolon, new()
        {
            string errorMessage = string.Concat("An error occured in HolonManager.LoadHolonForProviderTypeAsync attempting to load the holon for id ", id, " using the ", Enum.GetName(providerType), " provider. Reason: ");

            try
            {
                OASISResult<IOASISStorageProvider> providerResult = await ProviderManager.Instance.SetAndActivateCurrentStorageProviderAsync(providerType);

                if (!providerResult.IsError && providerResult.Result != null)
                {
                    //T convertedHolon = (T)Activator.CreateInstance(typeof(T)); //TODO: Need to find faster alternative to relfection... maybe JSON? 21/05/24: Not even sure why we had this line in the first place?! lol
                    var task = providerResult.Result.LoadHolonAsync(id, loadChildren, recursive, maxChildDepth, continueOnError, loadChildrenFromProvider, version);

                    if (await Task.WhenAny(task, Task.Delay(OASISDNA.OASIS.StorageProviders.ProviderMethodCallTimeOutSeconds * 1000)) == task)
                    {
                        if (task != null && task.Result != null)
                        {
                            if (task.Result.IsError || task.Result.Result == null)
                            {
                                if (string.IsNullOrEmpty(task.Result.Message))
                                    task.Result.Message = "No Holon Found.";

                                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from provider calling LoadHolonAsync: ", task.Result.Message), task.Result.DetailedMessage);
                            }
                            else
                            {
                                result.IsLoaded = true;
                                result.LoadedCount = 1;
                                result.Result = Mapper.MapBaseHolonPropertiesAndCreateT2IfNull(task.Result.Result, result.Result);
                            }
                        }
                        else
                            OASISErrorHandling.HandleWarning(ref result, $"{errorMessage}Result returned from provider calling LoadHolonAsync is null.");
                    }
                    else
                        OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Timeout occured from provider calling LoadHolonAsync."));
                }
                else
                    OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from ProviderManager.Instance.SetAndActivateCurrentStorageProviderAsync: ", providerResult.Message), providerResult.DetailedMessage);
            }
            catch (Exception ex)
            {
                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, ex.Message));
            }

            return result;
        }

        private OASISResult<IHolon> LoadHolonForProviderType(string providerKey, ProviderType providerType, OASISResult<IHolon> result, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, bool loadChildrenFromProvider = false, int version = 0)
        {
            string errorMessage = string.Concat("An error occured in HolonManager.LoadHolonForProviderType attempting to load the holon for providerKey ", providerKey, " using the ", Enum.GetName(providerType), " provider. Reason: ");

            try
            {
                OASISResult<IOASISStorageProvider> providerResult = ProviderManager.Instance.SetAndActivateCurrentStorageProvider(providerType);

                if (!providerResult.IsError && providerResult.Result != null)
                {
                    OASISResult<IHolon> holonResult = Task.Run(() => providerResult.Result.LoadHolon(providerKey, loadChildren, recursive, maxChildDepth, continueOnError, loadChildrenFromProvider, version)).WaitAsync(TimeSpan.FromSeconds(OASISDNA.OASIS.StorageProviders.ProviderMethodCallTimeOutSeconds)).Result;

                    if (holonResult != null)
                    {
                        if (holonResult.IsError || holonResult.Result == null)
                        {
                            if (string.IsNullOrEmpty(holonResult.Message))
                                holonResult.Message = "No Holon Found.";

                            OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from provider calling LoadHolon: ", holonResult.Message), result.DetailedMessage);
                        }
                        else
                        {
                            result.IsLoaded = true;
                            result.LoadedCount = 1;
                            result.Result = holonResult.Result;
                        }
                    }
                    else
                        OASISErrorHandling.HandleWarning(ref result, $"{errorMessage}Result returned from provider calling LoadHolon is null.");
                }
                else
                    OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from ProviderManager.Instance.SetAndActivateCurrentStorageProvider: ", providerResult.Message), providerResult.DetailedMessage);
            }
            catch (TimeoutException)
            {
                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Timeout occured from provider calling LoadHolon."));
            }
            catch (Exception ex)
            {
                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, ex.Message));
            }

            return result;
        }

        private OASISResult<T> LoadHolonForProviderType<T>(string providerKey, ProviderType providerType, OASISResult<T> result, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, bool loadChildrenFromProvider = false, int version = 0) where T : IHolon, new()
        {
            string errorMessage = string.Concat("An error occured in HolonManager.LoadHolonForProviderType<T> attempting to load the holon for providerKey ", providerKey, " using the ", Enum.GetName(providerType), " provider. Reason: ");

            try
            {
                OASISResult<IOASISStorageProvider> providerResult = ProviderManager.Instance.SetAndActivateCurrentStorageProvider(providerType);

                if (!providerResult.IsError && providerResult.Result != null)
                {
                    OASISResult<IHolon> holonResult = Task.Run(() => providerResult.Result.LoadHolon(providerKey, loadChildren, recursive, maxChildDepth, continueOnError, loadChildrenFromProvider, version)).WaitAsync(TimeSpan.FromSeconds(OASISDNA.OASIS.StorageProviders.ProviderMethodCallTimeOutSeconds)).Result;

                    if (holonResult != null)
                    {
                        if (holonResult.IsError || holonResult.Result == null)
                        {
                            if (string.IsNullOrEmpty(holonResult.Message))
                                holonResult.Message = "No Holon Found.";

                            OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from provider calling LoadHolon: ", holonResult.Message), result.DetailedMessage);
                        }
                        else
                        {
                            result.IsLoaded = true;
                            result.LoadedCount = 1;
                            result.Result = Mapper.MapBaseHolonPropertiesAndCreateT2IfNull(holonResult.Result, result.Result);
                        }
                    }
                    else
                        OASISErrorHandling.HandleWarning(ref result, $"{errorMessage}Result returned from provider calling LoadHolon is null.");
                }
                else
                    OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from ProviderManager.Instance.SetAndActivateCurrentStorageProvider: ", providerResult.Message), providerResult.DetailedMessage);
            }
            catch (TimeoutException)
            {
                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Timeout occured from provider calling LoadHolon."));
            }
            catch (Exception ex)
            {
                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, ex.Message));
            }

            return result;
        }

        private async Task<OASISResult<IHolon>> LoadHolonForProviderTypeAsync(string providerKey, ProviderType providerType, OASISResult<IHolon> result, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, bool loadChildrenFromProvider = false, int version = 0)
        {
            string errorMessage = string.Concat("An error occured in HolonManager.LoadHolonForProviderTypeAsync attempting to load the holon for providerKey ", providerKey, " using the ", Enum.GetName(providerType), " provider. Reason: ");

            try
            {
                OASISResult<IOASISStorageProvider> providerResult = await ProviderManager.Instance.SetAndActivateCurrentStorageProviderAsync(providerType);

                if (!providerResult.IsError && providerResult.Result != null)
                {
                    var task = providerResult.Result.LoadHolonAsync(providerKey, loadChildren, recursive, maxChildDepth, continueOnError, loadChildrenFromProvider, version);

                    if (await Task.WhenAny(task, Task.Delay(OASISDNA.OASIS.StorageProviders.ProviderMethodCallTimeOutSeconds * 1000)) == task)
                    {
                        if (task != null && task.Result != null)
                        {
                            if (task.Result.IsError || task.Result.Result == null)
                            {
                                if (string.IsNullOrEmpty(task.Result.Message))
                                    task.Result.Message = "No Holon Found.";

                                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from provider calling LoadHolonAsync: ", task.Result.Message), task.Result.DetailedMessage);
                            }
                            else
                            {
                                result.IsLoaded = true;
                                result.LoadedCount = 1;
                                result.Result = task.Result.Result;
                            }
                        }
                        else
                            OASISErrorHandling.HandleWarning(ref result, $"{errorMessage}Result returned from provider calling LoadHolonAsync is null.");
                    }
                    else
                        OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Timeout occured from provider calling LoadHolonAsync."));
                }
                else
                    OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from ProviderManager.Instance.SetAndActivateCurrentStorageProviderAsync: ", providerResult.Message), providerResult.DetailedMessage);
            }
            catch (Exception ex)
            {
                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, ex.Message));
            }

            return result;
        }

        private async Task<OASISResult<T>> LoadHolonForProviderTypeAsync<T>(string providerKey, ProviderType providerType, OASISResult<T> result, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, bool loadChildrenFromProvider = false, int version = 0) where T : IHolon, new()
        {
            string errorMessage = string.Concat("An error occured in HolonManager.LoadHolonForProviderTypeAsync<T> attempting to load the holon for providerKey ", providerKey, " using the ", Enum.GetName(providerType), " provider. Reason: ");

            try
            {
                OASISResult<IOASISStorageProvider> providerResult = await ProviderManager.Instance.SetAndActivateCurrentStorageProviderAsync(providerType);

                if (!providerResult.IsError && providerResult.Result != null)
                {
                    var task = providerResult.Result.LoadHolonAsync(providerKey, loadChildren, recursive, maxChildDepth, continueOnError, loadChildrenFromProvider, version);

                    if (await Task.WhenAny(task, Task.Delay(OASISDNA.OASIS.StorageProviders.ProviderMethodCallTimeOutSeconds * 1000)) == task)
                    {
                        if (task != null && task.Result != null)
                        {
                            if (task.Result.IsError || task.Result.Result == null)
                            {
                                if (string.IsNullOrEmpty(task.Result.Message))
                                    task.Result.Message = "No Holon Found.";

                                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from provider calling LoadHolonAsync: ", task.Result.Message), task.Result.DetailedMessage);
                            }
                            else
                            {
                                result.IsLoaded = true;
                                result.LoadedCount = 1;
                                result.Result = Mapper.MapBaseHolonPropertiesAndCreateT2IfNull(task.Result.Result, result.Result);
                            }
                        }
                        else
                            OASISErrorHandling.HandleWarning(ref result, $"{errorMessage}Result returned from provider calling LoadHolonAsync is null.");
                    }
                    else
                        OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Timeout occured from provider calling LoadHolonAsync."));
                }
                else
                    OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from ProviderManager.Instance.SetAndActivateCurrentStorageProviderAsync: ", providerResult.Message), providerResult.DetailedMessage);
            }
            catch (Exception ex)
            {
                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, ex.Message));
            }

            return result;
        }

        private OASISResult<IHolon> LoadHolonForProviderTypeByCustomKey(string customKey, ProviderType providerType, OASISResult<IHolon> result, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, bool loadChildrenFromProvider = false, int version = 0)
        {
            string errorMessage = string.Concat("An error occured in HolonManager.LoadHolonForProviderTypeByCustomKey attempting to load the holon for customKey ", customKey, " using the ", Enum.GetName(providerType), " provider. Reason: ");

            try
            {
                OASISResult<IOASISStorageProvider> providerResult = ProviderManager.Instance.SetAndActivateCurrentStorageProvider(providerType);

                if (!providerResult.IsError && providerResult.Result != null)
                {
                    OASISResult<IHolon> holonResult = Task.Run(() => providerResult.Result.LoadHolon(customKey, loadChildren, recursive, maxChildDepth, continueOnError, loadChildrenFromProvider, version)).WaitAsync(TimeSpan.FromSeconds(OASISDNA.OASIS.StorageProviders.ProviderMethodCallTimeOutSeconds)).Result;

                    if (holonResult != null)
                    {
                        if (holonResult.IsError || holonResult.Result == null)
                        {
                            if (string.IsNullOrEmpty(holonResult.Message))
                                holonResult.Message = "No Holon Found.";

                            OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from provider calling LoadHolon: ", holonResult.Message), result.DetailedMessage);
                        }
                        else
                        {
                            result.IsLoaded = true;
                            result.LoadedCount = 1;
                            result.Result = holonResult.Result;
                        }
                    }
                    else
                        OASISErrorHandling.HandleWarning(ref result, $"{errorMessage}Result returned from provider calling LoadHolon is null.");
                }
                else
                    OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from ProviderManager.Instance.SetAndActivateCurrentStorageProvider: ", providerResult.Message), providerResult.DetailedMessage);
            }
            catch (TimeoutException)
            {
                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Timeout occured from provider calling LoadHolon."));
            }
            catch (Exception ex)
            {
                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, ex.Message));
            }

            return result;
        }

        private OASISResult<T> LoadHolonForProviderTypeByCustomKey<T>(string customKey, ProviderType providerType, OASISResult<T> result, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, bool loadChildrenFromProvider = false, int version = 0) where T : IHolon, new()
        {
            string errorMessage = string.Concat("An error occured in HolonManager.LoadHolonForProviderTypeByCustomKey attempting to load the holon for customKey ", customKey, " using the ", Enum.GetName(providerType), " provider. Reason: ");

            try
            {
                OASISResult<IOASISStorageProvider> providerResult = ProviderManager.Instance.SetAndActivateCurrentStorageProvider(providerType);

                if (!providerResult.IsError && providerResult.Result != null)
                {
                    OASISResult<IHolon> holonResult = Task.Run(() => providerResult.Result.LoadHolon(customKey, loadChildren, recursive, maxChildDepth, continueOnError, loadChildrenFromProvider, version)).WaitAsync(TimeSpan.FromSeconds(OASISDNA.OASIS.StorageProviders.ProviderMethodCallTimeOutSeconds)).Result;

                    if (holonResult != null)
                    {
                        if (holonResult.IsError || holonResult.Result == null)
                        {
                            if (string.IsNullOrEmpty(holonResult.Message))
                                holonResult.Message = "No Holon Found.";

                            OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from provider calling LoadHolon: ", holonResult.Message), result.DetailedMessage);
                        }
                        else
                        {
                            result.IsLoaded = true;
                            result.LoadedCount = 1;
                            result.Result = Mapper.MapBaseHolonPropertiesAndCreateT2IfNull(holonResult.Result, result.Result);
                        }
                    }
                    else
                        OASISErrorHandling.HandleWarning(ref result, $"{errorMessage}Result returned from provider calling LoadHolon is null.");
                }
                else
                    OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from ProviderManager.Instance.SetAndActivateCurrentStorageProvider: ", providerResult.Message), providerResult.DetailedMessage);
            }
            catch (TimeoutException)
            {
                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Timeout occured from provider calling LoadHolon."));
            }
            catch (Exception ex)
            {
                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, ex.Message));
            }

            return result;
        }

        private async Task<OASISResult<IHolon>> LoadHolonForProviderTypeByCustomKeyAsync(string customKey, ProviderType providerType, OASISResult<IHolon> result, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, bool loadChildrenFromProvider = false, int version = 0)
        {
            string errorMessage = string.Concat("An error occured in HolonManager.LoadHolonForProviderTypeAsync attempting to load the holon for customKey ", customKey, " using the ", Enum.GetName(providerType), " provider. Reason: ");

            try
            {
                OASISResult<IOASISStorageProvider> providerResult = await ProviderManager.Instance.SetAndActivateCurrentStorageProviderAsync(providerType);

                if (!providerResult.IsError && providerResult.Result != null)
                {
                    var task = providerResult.Result.LoadHolonAsync(customKey, loadChildren, recursive, maxChildDepth, continueOnError, loadChildrenFromProvider, version);

                    if (await Task.WhenAny(task, Task.Delay(OASISDNA.OASIS.StorageProviders.ProviderMethodCallTimeOutSeconds * 1000)) == task)
                    {
                        if (task != null && task.Result != null)
                        {
                            if (task.Result.IsError || task.Result.Result == null)
                            {
                                if (string.IsNullOrEmpty(task.Result.Message))
                                    task.Result.Message = "No Holon Found.";

                                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from provider calling LoadHolonAsync: ", task.Result.Message), task.Result.DetailedMessage);
                            }
                            else
                            {
                                result.IsLoaded = true;
                                result.LoadedCount = 1;
                                result.Result = task.Result.Result;
                            }
                        }
                        else
                            OASISErrorHandling.HandleWarning(ref result, $"{errorMessage}Result returned from provider calling LoadHolonAsync is null.");
                    }
                    else
                        OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Timeout occured from provider calling LoadHolonAsync."));
                }
                else
                    OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from ProviderManager.Instance.SetAndActivateCurrentStorageProviderAsync: ", providerResult.Message), providerResult.DetailedMessage);
            }
            catch (Exception ex)
            {
                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, ex.Message));
            }

            return result;
        }

        private async Task<OASISResult<T>> LoadHolonForProviderTypeByCustomKeyAsync<T>(string customKey, ProviderType providerType, OASISResult<T> result, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, bool loadChildrenFromProvider = false, int version = 0) where T : IHolon, new()
        {
            string errorMessage = string.Concat("An error occured in HolonManager.LoadHolonForProviderTypeAsync<T> attempting to load the holon for customKey ", customKey, " using the ", Enum.GetName(providerType), " provider. Reason: ");

            try
            {
                OASISResult<IOASISStorageProvider> providerResult = await ProviderManager.Instance.SetAndActivateCurrentStorageProviderAsync(providerType);

                if (!providerResult.IsError && providerResult.Result != null)
                {
                    var task = providerResult.Result.LoadHolonAsync(customKey, loadChildren, recursive, maxChildDepth, continueOnError, loadChildrenFromProvider, version);

                    if (await Task.WhenAny(task, Task.Delay(OASISDNA.OASIS.StorageProviders.ProviderMethodCallTimeOutSeconds * 1000)) == task)
                    {
                        if (task != null && task.Result != null)
                        {
                            if (task.Result.IsError || task.Result.Result == null)
                            {
                                if (string.IsNullOrEmpty(task.Result.Message))
                                    task.Result.Message = "No Holon Found.";

                                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from provider calling LoadHolonAsync: ", task.Result.Message), task.Result.DetailedMessage);
                            }
                            else
                            {
                                result.IsLoaded = true;
                                result.LoadedCount = 1;
                                result.Result = Mapper.MapBaseHolonPropertiesAndCreateT2IfNull(task.Result.Result, result.Result);
                            }
                        }
                        else
                            OASISErrorHandling.HandleWarning(ref result, $"{errorMessage}Result returned from provider calling LoadHolonAsync is null.");
                    }
                    else
                        OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Timeout occured from provider calling LoadHolonAsync."));
                }
                else
                    OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from ProviderManager.Instance.SetAndActivateCurrentStorageProviderAsync: ", providerResult.Message), providerResult.DetailedMessage);
            }
            catch (Exception ex)
            {
                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, ex.Message));
            }

            return result;
        }

        private OASISResult<IHolon> LoadHolonForProviderTypeByMetaData(string metaKey, string metaValue, ProviderType providerType, OASISResult<IHolon> result, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, bool loadChildrenFromProvider = false, int version = 0)
        {
            string errorMessage = string.Concat("An error occured in HolonManager.LoadHolonForProviderTypeByCustomKey attempting to load the holon for metaKey ", metaKey, " and metaValue ", metaValue, " using the ", Enum.GetName(providerType), " provider. Reason: ");

            try
            {
                OASISResult<IOASISStorageProvider> providerResult = ProviderManager.Instance.SetAndActivateCurrentStorageProvider(providerType);

                if (!providerResult.IsError && providerResult.Result != null)
                {
                    OASISResult<IHolon> holonResult = Task.Run(() => providerResult.Result.LoadHolonByMetaData(metaKey, metaValue, loadChildren, recursive, maxChildDepth, continueOnError, loadChildrenFromProvider, version)).WaitAsync(TimeSpan.FromSeconds(OASISDNA.OASIS.StorageProviders.ProviderMethodCallTimeOutSeconds)).Result;

                    if (holonResult != null)
                    {
                        if (holonResult.IsError || holonResult.Result == null)
                        {
                            if (string.IsNullOrEmpty(holonResult.Message))
                                holonResult.Message = "No Holon Found.";

                            OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from provider calling LoadHolon: ", holonResult.Message), result.DetailedMessage);
                        }
                        else
                        {
                            result.IsLoaded = true;
                            result.LoadedCount = 1;
                            result.Result = holonResult.Result;
                        }
                    }
                    else
                        OASISErrorHandling.HandleWarning(ref result, $"{errorMessage}Result returned from provider calling LoadHolon is null.");
                }
                else
                    OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from ProviderManager.Instance.SetAndActivateCurrentStorageProvider: ", providerResult.Message), providerResult.DetailedMessage);
            }
            catch (TimeoutException)
            {
                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Timeout occured from provider calling LoadHolon."));
            }
            catch (Exception ex)
            {
                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, ex.Message));
            }

            return result;
        }

        private OASISResult<T> LoadHolonForProviderTypeByMetaData<T>(string metaKey, string metaValue, ProviderType providerType, OASISResult<T> result, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, bool loadChildrenFromProvider = false, int version = 0) where T : IHolon, new()
        {
            string errorMessage = string.Concat("An error occured in HolonManager.LoadHolonForProviderTypeByCustomKey attempting to load the holon for metaKey ", metaKey, " and metaValue ", metaValue, " using the ", Enum.GetName(providerType), " provider. Reason: ");

            try
            {
                OASISResult<IOASISStorageProvider> providerResult = ProviderManager.Instance.SetAndActivateCurrentStorageProvider(providerType);

                if (!providerResult.IsError && providerResult.Result != null)
                {
                    OASISResult<IHolon> holonResult = Task.Run(() => providerResult.Result.LoadHolonByMetaData(metaKey, metaValue, loadChildren, recursive, maxChildDepth, continueOnError, loadChildrenFromProvider, version)).WaitAsync(TimeSpan.FromSeconds(OASISDNA.OASIS.StorageProviders.ProviderMethodCallTimeOutSeconds)).Result;

                    if (holonResult != null)
                    {
                        if (holonResult.IsError || holonResult.Result == null)
                        {
                            if (string.IsNullOrEmpty(holonResult.Message))
                                holonResult.Message = "No Holon Found.";

                            OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from provider calling LoadHolon: ", holonResult.Message), result.DetailedMessage);
                        }
                        else
                        {
                            result.IsLoaded = true;
                            result.LoadedCount = 1;
                            result.Result = Mapper.MapBaseHolonPropertiesAndCreateT2IfNull(holonResult.Result, result.Result);
                        }
                    }
                    else
                        OASISErrorHandling.HandleWarning(ref result, $"{errorMessage}Result returned from provider calling LoadHolon is null.");
                }
                else
                    OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from ProviderManager.Instance.SetAndActivateCurrentStorageProvider: ", providerResult.Message), providerResult.DetailedMessage);
            }
            catch (TimeoutException)
            {
                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Timeout occured from provider calling LoadHolon."));
            }
            catch (Exception ex)
            {
                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, ex.Message));
            }

            return result;
        }

        private async Task<OASISResult<IHolon>> LoadHolonForProviderTypeByMetaDataAsync(string metaKey, string metaValue, ProviderType providerType, OASISResult<IHolon> result, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, bool loadChildrenFromProvider = false, int version = 0)
        {
            string errorMessage = string.Concat("An error occured in HolonManager.LoadHolonForProviderTypeByMetaDataAsync attempting to load the holon for metaKey ", metaKey, " and metaValue ", metaValue, " using the ", Enum.GetName(providerType), " provider. Reason: ");

            try
            {
                OASISResult<IOASISStorageProvider> providerResult = await ProviderManager.Instance.SetAndActivateCurrentStorageProviderAsync(providerType);

                if (!providerResult.IsError && providerResult.Result != null)
                {
                    var task = providerResult.Result.LoadHolonByMetaDataAsync(metaKey, metaValue, loadChildren, recursive, maxChildDepth, continueOnError, loadChildrenFromProvider, version);

                    if (await Task.WhenAny(task, Task.Delay(OASISDNA.OASIS.StorageProviders.ProviderMethodCallTimeOutSeconds * 1000)) == task)
                    {
                        if (task != null && task.Result != null)
                        {
                            if (task.Result.IsError || task.Result.Result == null)
                            {
                                if (string.IsNullOrEmpty(task.Result.Message))
                                    task.Result.Message = "No Holon Found.";

                                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from provider calling LoadHolonAsync: ", task.Result.Message), task.Result.DetailedMessage);
                            }
                            else
                            {
                                result.IsLoaded = true;
                                result.LoadedCount = 1;
                                result.Result = task.Result.Result;
                            }
                        }
                        else
                            OASISErrorHandling.HandleWarning(ref result, $"{errorMessage}Result returned from provider calling LoadHolonAsync is null.");
                    }
                    else
                        OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Timeout occured from provider calling LoadHolonAsync."));
                }
                else
                    OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from ProviderManager.Instance.SetAndActivateCurrentStorageProviderAsync: ", providerResult.Message), providerResult.DetailedMessage);
            }
            catch (Exception ex)
            {
                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, ex.Message));
            }

            return result;
        }

        private async Task<OASISResult<T>> LoadHolonForProviderTypeByMetaDataAsync<T>(string metaKey, string metaValue, ProviderType providerType, OASISResult<T> result, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, bool loadChildrenFromProvider = false, int version = 0) where T : IHolon, new()
        {
            string errorMessage = string.Concat("An error occured in HolonManager.LoadHolonForProviderTypeByMetaDataAsync attempting to load the holon for metaKey ", metaKey, " and metaValue ", metaValue, " using the ", Enum.GetName(providerType), " provider. Reason: ");

            try
            {
                OASISResult<IOASISStorageProvider> providerResult = await ProviderManager.Instance.SetAndActivateCurrentStorageProviderAsync(providerType);

                if (!providerResult.IsError && providerResult.Result != null)
                {
                    var task = providerResult.Result.LoadHolonByMetaDataAsync(metaKey, metaValue, loadChildren, recursive, maxChildDepth, continueOnError, loadChildrenFromProvider, version);

                    if (await Task.WhenAny(task, Task.Delay(OASISDNA.OASIS.StorageProviders.ProviderMethodCallTimeOutSeconds * 1000)) == task)
                    {
                        if (task != null && task.Result != null)
                        {
                            if (task.Result.IsError || task.Result.Result == null)
                            {
                                if (string.IsNullOrEmpty(task.Result.Message))
                                    task.Result.Message = "No Holon Found.";

                                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from provider calling LoadHolonAsync: ", task.Result.Message), task.Result.DetailedMessage);
                            }
                            else
                            {
                                result.IsLoaded = true;
                                result.LoadedCount = 1;
                                result.Result = Mapper.MapBaseHolonPropertiesAndCreateT2IfNull(task.Result.Result, result.Result);
                            }
                        }
                        else
                            OASISErrorHandling.HandleWarning(ref result, $"{errorMessage}Result returned from provider calling LoadHolonAsync is null.");
                    }
                    else
                        OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Timeout occured from provider calling LoadHolonAsync."));
                }
                else
                    OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, "Error returned from ProviderManager.Instance.SetAndActivateCurrentStorageProviderAsync: ", providerResult.Message), providerResult.DetailedMessage);
            }
            catch (Exception ex)
            {
                OASISErrorHandling.HandleWarning(ref result, string.Concat(errorMessage, ex.Message));
            }

            return result;
        }
    }
}