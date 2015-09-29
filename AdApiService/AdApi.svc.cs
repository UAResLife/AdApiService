using ApiLogging;
using ApiSecurity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace AdApiService
{

    public class AdApi : IAdApi
    {
        
        /// <summary>
        /// AddMember
        /// 
        /// Adds the specified principal to the specified group.
        /// </summary>
        /// <param name="apikey">The key used to authenticate the request.</param>
        /// <param name="member">A unique identifier for a user or group, for example, SamAccountName.</param>
        /// <param name="group">A unique identifier for a group, for example, SamAccountName.</param>
        /// <returns>
        /// A Boolean. True if the member was added to the group, false otherwise. This can return
        /// false if the member or group does not exist.
        /// </returns>
        public Boolean AddMember(string apikey, string member, string group)
        {
            if (ApiAccess.IsAllowed(apikey) && (!IsProtectedObject(group)))
            {
                try
                {
                    return AdToolkit.AddMember(member, group);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// CopyUser
        /// 
        /// Copies a user account to the specified location. A random password will be assigned to the user
        /// and the account will be disabbled. Before the account can be used it will need to be
        /// unlocked and have its password reset.
        /// 
        /// The following attributes are copied from the template:
        /// description, co, company, countryCode, department, l, physicalDeliveryOfficeName, postalCode,
        /// profilePath (modified for the new user), st, streetAddress.
        /// 
        /// It also copies the group membership of the template.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="firstName">The givenName of the new user.</param>
        /// <param name="lastName">The surName (sn) of the new user.</param>
        /// <param name="samAccountName">The new logon name. This must be unique or this method will fail.</param>
        /// <param name="location">The distinguishedName of the OU where the new user should be created.</param>
        /// <param name="templateSamAccountName"></param>
        /// <returns></returns>
        public AdUser CopyUser(string apikey, string firstName, string lastName, string samAccountName, string location, string templateSamAccountName)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    HttpContext.Current.Server.ScriptTimeout = 300;
                    return new AdUser(AdToolkit.CopyUser(firstName, lastName, samAccountName, location, templateSamAccountName));
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// CreateComputer
        /// 
        /// Creates a computer with the specified name and description in the specified location.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The name of the computer.</param>
        /// <param name="location">The distinguished name of the OU to create the computer in.</param>
        /// <param name="description">The value that will be assigned to the new computer's description property.</param>
        /// <returns>True if created, false otherwise.</returns>
        public AdComputer CreateComputer(string apikey, string name, string location, string description)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return new AdComputer(AdToolkit.CreateComputer(name, location, description));
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// CreateGroup
        /// 
        /// Creates a new group in the specified location.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The name of the group will be used for the name, display name, and SamAccountName.</param>
        /// <param name="location">The distinguished name of the OU in which the group should be created.</param>
        /// <param name="description">The description of the group.</param>
        /// <param name="groupScope">The GroupScope. One of Global, Local, or Univeral.</param>
        /// <param name="isSecurityGroup"></param>
        /// <returns>An AdGroup object representing the new group.</returns>
        public AdGroup CreateGroup(string apikey, string name, string location, string description, string groupScope, string isSecurityGroup)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    // convert groupScope to a GroupScope
                    GroupScope scope = GroupScope.Universal;
                    if (groupScope.ToUpper() == GroupScope.Global.ToString().ToUpper())
                    {
                        scope = GroupScope.Global;
                    }
                    else if (groupScope.ToUpper() == GroupScope.Local.ToString().ToUpper())
                    {
                        scope = GroupScope.Local;
                    }
                    // convert isSecurityGroup to a bool
                    bool isSecGrp = false;
                    if (string.Equals(isSecurityGroup, "true", StringComparison.CurrentCultureIgnoreCase))
                    {
                        isSecGrp = true;
                    }
                    return new AdGroup(AdToolkit.CreateGroup(name, location, description, scope, isSecGrp));
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// CreateUser
        /// 
        /// Creates a new user account in the specified location. The new account will be disabled.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="firstName">The givenName of the new user.</param>
        /// <param name="lastName">The surname of the new user.</param>
        /// <param name="samAccountName">the SamAccountName of the new user.</param>
        /// <param name="location">The distinguished name of the OU in which the new user should be created.</param>
        /// <returns>An AdUser account representing the new user.</returns>
        public AdUser CreateUser(string apikey, string firstName, string lastName, string samAccountName, string location)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return new AdUser(AdToolkit.CreateUser(firstName, lastName, samAccountName, location));
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// DeleteComputer
        /// 
        /// Deletes the specified computer.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier of the computer.</param>
        /// <returns>True if the computer was deleted, false otherwise.</returns>
        public Boolean DeleteComputer(string apikey, string name)
        {
            if (ApiAccess.IsAllowed(apikey) && (!IsProtectedObject(name)))
            {
                try
                {
                    return AdToolkit.DeleteComputer(name);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// DeleteGroup
        /// 
        /// Deletes the specified group.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier of the group.</param>
        /// <returns>True if the group was deleted, false otherwise.</returns>
        public Boolean DeleteGroup(string apikey, string name)
        {
            if (ApiAccess.IsAllowed(apikey) && (!IsProtectedObject(name)))
            {
                try
                {
                    return AdToolkit.DeleteGroup(name);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// DeleteUser
        /// 
        /// Deletes the specified user.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier of the user.</param>
        /// <returns>True if the user was deleted, false otherwise.</returns>
        public Boolean DeleteUser(string apikey, string name)
        {
            if (ApiAccess.IsAllowed(apikey) && (!IsProtectedObject(name)))
            {
                try
                {
                    return AdToolkit.DeleteUser(name);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// DisableUser
        /// 
        /// Disables the specified user.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier of the user.</param>
        /// <returns>True if the user was deleted, false otherwise.</returns>
        public Boolean DisableUser(string apikey, string name)
        {
            if (ApiAccess.IsAllowed(apikey) && (!IsProtectedObject(name)))
            {
                try
                {
                    return AdToolkit.DisableUser(name);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// EnableUser
        /// 
        /// Sets the enabled flag on the specified user to true.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier for the user.</param>
        /// <returns>True if enabled, false otherwise.</returns>
        public Boolean EnableUser(string apikey, string name)
        {
            if (ApiAccess.IsAllowed(apikey) && (!IsProtectedObject(name)))
            {
                try
                {
                    return AdToolkit.EnableUser(name);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// Exists
        /// 
        /// Checks to see if the specified item exists. The item can be a user, group, computer, or OU.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="item">The item to test. This can be a SamAccountName, UserPrincipalName, or DistinguishedName.</param>
        /// <returns>True if the item exists, false otherwise.</returns>
        public Boolean Exists(string apikey, string item)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return AdToolkit.Exists(item);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// ExpirePassword
        /// 
        /// Expires the password of the specified principal. The principal can be either a user or computer.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The name of the principal that should have its password expired.</param>
        /// <returns>True if the password was expired, false otherwise.</returns>
        public Boolean ExpirePassword(string apikey, string name)
        {
            if (ApiAccess.IsAllowed(apikey) && (!IsProtectedObject(name)))
            {
                try
                {
                    return AdToolkit.ExpirePassword(name);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// GetComputer
        /// 
        /// Returns the attributes of the specified computer.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">A unique identifier for the computer, either a Name, SamAccountName, DistinguishedName, or UserPrincipalName.</param>
        /// <returns>An AdComputer object.</returns>
        public AdComputer GetComputer(string apikey, string name)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return new AdComputer(AdToolkit.GetComputer(name));
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// GetComputersInGroup
        /// 
        /// Returns a list of AdComputer objects that are members of the specified group.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="group">The unique identifier of the group.</param>
        /// <returns>A list of AdComputer objects.</returns>
        public List<AdComputer> GetComputersInGroup(string apikey, string group)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return AdComputer.GetAdComputers(AdToolkit.GetComputersInGroup(group));
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// GetCurrentDomain
        /// 
        /// Returns the Full domain name of the current context, i.e., the context of the accout the code is running under.
        /// In IIS this would be the Application Pool identity.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <returns>A string that is the full domain name of the current PrincipalContext.</returns>
        public string GetCurrentDomain(string apikey)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return AdToolkit.GetCurrentDomain();
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// GetDistinctUserProperty
        /// 
        /// Returns the distinct values of the specified property found on any user in the specified OU.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="property">Distinct values of this property will be returned.</param>
        /// <param name="location">The distinguished name of the OU to look in.</param>
        /// <returns>A list of strings that are sorted distinct values of the specified property.</returns>
        public List<string> GetDistinctUserProperty(string apikey, string property, string location)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return AdToolkit.GetDistinctUserProperty(property, location);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// GetDistinguishedName
        /// 
        /// Returns the DistinguishedName associated with the specified Principal, which is a user, computer, group, or OU.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier for the Principal.</param>
        /// <returns>A string that is the distinguishedName of the specified Principal.</returns>
        public string GetDistinguishedName(string apikey, string name)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return AdToolkit.GetDistinguishedName(name);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// GetDomainContollers
        /// 
        /// Returns a list of all domain controllers in the current domain.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <returns>
        /// A list of strings that are the fully qualified domain names of all the domain controllers in the current domain.
        /// </returns>
        public List<string> GetDomainControllers(string apikey)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return AdToolkit.GetDomainControllers();
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// GetForestDomains
        /// 
        /// Returns a list of all the domains in the Forest.
        /// </summary>
        /// <param name="apikey">The apikey used to authorize the request.</param>
        /// <returns>A list of strings that are the domains in the Forest.</returns>
        public List<string> GetForestDomains(string apikey)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return AdToolkit.GetForestDomains();
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// GetFullDomainName
        /// 
        /// Returns the full domain name based on the friendly name. For example, in the my.mega.corp domain passing in
        /// 'my' would return 'my.mega.corp'.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="domain">The friendly domain name. In my.domain it would be 'my'.</param>
        /// <returns>A string that is the full domain name of the specified friendly domain name.</returns>
        public string GetFullDomainName(string apikey, string domain)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return AdToolkit.GetFullDomainName(domain);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);

        }

        /// <summary>
        /// GetGlobalCatalogServers
        /// 
        /// Returns a list of all domain controllers that are Global Catalog servers.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <returns>
        /// A list of strings that are the fully qualified domain names of the Global Catalog servers in the current domain.
        /// </returns>
        public List<string> GetGlobalCatalogServers(string apikey)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return AdToolkit.GetGlobalCatalogServers();
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// GetGroup
        /// 
        /// Returns the attributes and values of the specified group.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">A unique identifier for the group.</param>
        /// <returns>An AdGroup object.</returns>
        public AdGroup GetGroup(string apikey, string name)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return new AdGroup(AdToolkit.GetGroup(name));
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// GetGroups
        /// 
        /// Returns a list of groups in the specified OU.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="location">The distinguished name of the OU in which to look for groups.</param>
        /// <returns>A list of AdGroup objects.</returns>
        public List<AdGroup> GetGroups(string apikey, string location)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return AdGroup.GetAdGroups(AdToolkit.GetGroups(location));
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// GetGroupsInGroup
        /// 
        /// Returns a list of groups that are members of the specified group.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="group">The unique identifier of the group.</param>
        /// <returns>A list of AdGroup objects.</returns>
        public List<AdGroup> GetGroupsInGroup(string apikey, string group)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return AdGroup.GetAdGroups(AdToolkit.GetGroupsInGroup(group));
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// GetGroupMembership
        /// 
        /// Returns a list of strings that are the SamAccountNames of the groups the specified Principal is a member of.
        /// The Principal is a user, group, or computer.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifer of the principal.</param>
        /// <returns>A list of strings that are SamAccountNames of the groups the specified Principal is a member of.</returns>
        public List<string> GetGroupMembership(string apikey, string name)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return AdGroup.GetAdGroupsSamAccountNames(AdToolkit.GetGroupMembership(name));
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// GetOuItems
        /// 
        /// Returns a list of distinguished names that are children of the specified OU.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="location">The distinguished name of the specified OU.</param>
        /// <returns>A list of strings that are the distinguished names of the children of the specified OU.</returns>
        public List<string> GetItems(string apikey, string location)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return AdToolkit.GetItems(location);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// GetMembers
        /// 
        /// Returns the members of the specified group. Returned items may be a user, computer, or group.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="group">The unique identifier for the group.</param>
        /// <returns>A list of AdMember objects.</returns>
        public List<AdMember> GetMembers(string apikey, string group)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return AdMember.GetAdMembers(AdToolkit.GetMembers(group));
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// GetProperty
        /// 
        /// Returns the value of the specified property of the specified Principal.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier of the Principal. The principal can be a user, group, or computer.</param>
        /// <param name="property">The name of the property.</param>
        /// <returns>A string representing the value of the specified property of the specified Principal.</returns>
        public string GetProperty(string apikey, string name, string property)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return AdToolkit.GetProperty(name, property);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// GetRandomPassword
        /// 
        /// Returns a password that is 12 characters long and has at least 3 special chars.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <returns>A string that is a random password 12 characters long.</returns>
        public string GetRandomPassword(string apikey)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return AdToolkit.GetRandomPassword();
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// GetRootContainer
        /// 
        /// Returns the full domain name of the current domain reformatted as the root ldap level. For example, if the 
        /// current domain is my.mega.corp, DC=my,DC=mega,DC=corp is returned.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <returns>A string that is the top level ldap path.</returns>
        public string GetRootContainer(string apikey)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return AdToolkit.GetRootContainer();
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// GetSamAccountName
        /// 
        /// Returns the SamAccountName of the Principal identified by the specified name which can be a distinguished name, 
        /// UserPrincipalName, or SamAccountName. The Princiapl must be either a user, computer, or group. If passing in a
        /// distinguished name keep in mind that some distinguished  names can't be passed in a URL, for example, if a user's 
        /// name is in lastname, firstname format the distinguished name will have a backslash to escape the comma.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier of the Principal whose SamAccountName should be returned.</param>
        /// <returns>A string that is the SamAccountName of the Principal specified by the DistinguishedName.</returns>
        public string GetSamAccountName(string apikey, string name)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return AdToolkit.GetSamAccountName(name);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// GetUsedAttributes
        /// 
        /// Returns the attributes names and values of any attributes that are not empty. Some attributes are represented
        /// by the object's tostring value because microsoft can't use a regular DateTime like everyone else.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier of the Principal whose atttributes should be returned.</param>
        /// <returns>A dictionary where the keys are the attribute names and the values are the attribute values.</returns>
        public SortedDictionary<string, string> GetUsedAttributes(string apikey, string name)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return AdToolkit.GetUsedAttributes(name);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// GetUser
        /// 
        /// Returns the property names and values of the specified UserPrincipal.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier of the UserPrincipal.</param>
        /// <returns>An AdUser object.</returns>
        public AdUser GetUser(string apikey, string name)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return new AdUser(AdToolkit.GetUser(name));
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// GetUserByEmail
        /// 
        /// Returns an AdUser object associated with the specified email account. This method
        /// only returns users from OUs that have ",Users," in their distinguished names.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="email">The email address that identifies a user account.</param>
        /// <returns>An AdUser object with an email address that is the first match to the specified email address.</returns>
        public AdUser GetUserByEmail(string apikey, string email)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return new AdUser(AdToolkit.GetUserByEmail(email));
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// GetUsers
        /// 
        /// Returns all users in the specified OU.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="location">
        /// The path, i.e., the distinguished name, of the OU from in which the user accounts should exist.
        /// </param>
        /// <returns>A list of AdUser objects.</returns>
        public List<AdUser> GetUsers(string apikey, string location)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return AdUser.GetAdUsers(AdToolkit.GetUsers(location));
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// GetUsersInGroup
        /// 
        /// Returns a list of users that are members of the specified group.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="group"></param>
        /// <returns>A list of AdUser objects.</returns>
        public List<AdUser> GetUsersInGroup(string apikey, string group)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return AdUser.GetAdUsers(AdToolkit.GetUsersInGroup(group));
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// GetUserSelect
        /// 
        /// Returns the specified properties of the specified user. The select parameter is a dot-delimited list
        /// of properties, for example, to return just the SamAccountName, display name, and description the value
        /// of the select parameter would be, "samaccountname.displayname.description".
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier of the user.</param>
        /// <param name="select">A dot-delimited list of properties that should be returned.</param>
        /// <returns>
        /// A list of dictionaries where the keys are the names of the properties and the values are the property values.
        /// </returns>
        public List<SortedDictionary<string, string>> GetUserSelect(string apikey, string name, string select)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    List<string> properties = select.Split('.').ToList();
                    return AdToolkit.GetUser(name, properties);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// IsLockedOut
        /// 
        /// Returns true if the specified Principal, a user or computer, is locked out.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier of the user.</param>
        /// <returns>True if the user account is locked, false otherwise.</returns>
        public Boolean IsLocked(string apikey, string name)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return AdToolkit.IsLockedOut(name);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// IsMemberOf
        /// 
        /// Returns true if the specified principal, i.e., user, group, or computer, is a member of
        /// the specified group.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="principal">The unique identifier that corresponds to the Principal.</param>
        /// <param name="group">The unique identifier that corresponds to the group.</param>
        /// <returns>True if the principal is a member of the group, false otherwise.</returns>
        public Boolean IsMember(string apikey, string principal, string group)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return AdToolkit.IsMember(principal, group);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);

        }

        /// <summary>
        /// Tests if the specified Active Directory object is in the list of protected objects
        /// in the AppSettings section of the Web.config file.
        /// </summary>
        /// <param name="adObject">
        /// The Active Directory object to test. It can be a user, group, or computer
        /// or a comma separated list of any or all of those.
        /// </param>
        /// <returns>
        /// True if the specified object is in the list of protected objects, false otherwise.
        /// </returns>
        private Boolean IsProtectedObject(string adObject)
        {
            string protected_objects = ConfigurationManager.AppSettings.Get("protected_objects");
            return ApiAccess.AccessContains(protected_objects, AdToolkit.GetSamAccountName(adObject));
        }

        /// <summary>
        /// Move
        /// 
        /// Moves the specified Principal to the specified OU.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifer of the Principal.</param>
        /// <param name="location">The path, i.e., distinguished name of the OU that the Principal should be moved to.</param>
        /// <returns>True if the principal was moved, false otherwise.</returns>
        public Boolean Move(string apikey, string name, string location)
        {
            if (ApiAccess.IsAllowed(apikey) && (!IsProtectedObject(name)))
            {
                try
                {
                    return AdToolkit.Move(name, location);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// RemoveMember
        /// 
        /// Removes the specified member from the specified group.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="member">The member to remove.</param>
        /// <param name="group">The group from which the member should be removed.</param>
        /// <returns>A boolean, true if the member was removed from the group, false otherwise.</returns>
        public Boolean RemoveMember(string apikey, string member, string group)
        {
            if (ApiAccess.IsAllowed(apikey) && (!IsProtectedObject(member)) && (!IsProtectedObject(group)))
            {
                try
                {
                    return AdToolkit.RemoveMember(member, group);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// SetManager
        /// 
        /// Sets the Manager property of the specified user to the distinguished name of the specified manager.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="user">The unique identifier for the user.</param>
        /// <param name="manager">The unique identifier for the manager</param>
        /// <returns>A Boolean, true if the Manager attribute was updated, false otherwise.</returns>
        public Boolean SetManager(string apikey, string user, string manager)
        {
            if (ApiAccess.IsAllowed(apikey) && (!IsProtectedObject(user)))
            {
                try
                {
                    string property = "Manager";
                    //string value = AdToolkit.GetDistinguishedName(user);
                    return AdToolkit.SetProperty(user, property, manager);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// SetPassword
        /// 
        /// Sets the password of the specified user to the specified password.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier of the user.</param>
        /// <param name="password">The new password for the user.</param>
        /// <returns>True if success, false otherwise.</returns>
        public Boolean SetPassword(string apikey, string name, string password)
        {
            if (ApiAccess.IsAllowed(apikey) && (!IsProtectedObject(name)))
            {
                try
                {
                    return AdToolkit.SetPassword(name, password);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// SetProperty
        /// 
        /// Sets the specified property to the specified value on the specified Principal.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier for the Principal.</param>
        /// <param name="property">The name of the property.</param>
        /// <param name="value">The value to assign to the property.</param>
        /// <returns>True if success, false otherwise.</returns>
        public Boolean SetProperty(string apikey, string name, string property, string value)
        {
            if (ApiAccess.IsAllowed(apikey) && (!IsProtectedObject(name)))
            {
                try
                {
                    return AdToolkit.SetProperty(name, property, value);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// UnlockAccount
        /// 
        /// Unlocks the specified user or computer account.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier for the computer or user.</param>
        /// <returns>True if success, false otherwise.</returns>
        public Boolean Unlock(string apikey, string name)
        {
            if (ApiAccess.IsAllowed(apikey) && (!IsProtectedObject(name)))
            {
                try
                {
                    return AdToolkit.Unlock(name);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// ValidateCredentials
        /// 
        /// Tests that the specified password is valid for the specified user.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="username">The SamAccountName/logon name/username of the user.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>True if the credentials are valid, false otherwise.</returns>
        public Boolean ValidateCredentials(string apikey, string username, string password)
        {
            if (ApiAccess.IsAllowed(apikey))
            {
                try
                {
                    return AdToolkit.ValidateCredentials(username, password);
                }
                catch (Exception e)
                {
                    EventLogger.Instance.WriteError(e.Message + "\n" + e.StackTrace);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }


    } // end class AdApi

} // end namespace AdApiService
