using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace AdApiService
{

    [ServiceContract]
    public interface IAdApi
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
        [OperationContract(Name = "AddMember")]
        [WebInvoke(Method = "PUT", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/group/add/{member}/{group}")]
        Boolean AddMember(string apikey, string member, string group);

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
        /// <returns>An AdUser object representing the new user.</returns>
        [OperationContract(Name = "CopyUser")]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/user/copy/{firstName}/{lastName}/{samAccountName}/{location}/{templateSamAccountName}")]
        AdUser CopyUser(string apikey, string firstName, string lastName, string samAccountName, string location, string templateSamAccountName);

        /// <summary>
        /// CreateComputer
        /// 
        /// Creates a computer with the specified name and description in the specified location.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The name of the computer.</param>
        /// <param name="location">The distinguished name of the OU to create the computer in.</param>
        /// <param name="description">The value that will be assigned to the new computer's description property.</param>
        /// <returns>The AdComputer object of the new computer.</returns>
        [OperationContract(Name = "CreateComputer")]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/computer/create/{name}/{location}/{description}")]
        AdComputer CreateComputer(string apikey, string name, string location, string description);

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
        [OperationContract(Name = "CreateGroup")]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/group/create/{name}/{location}/{description}/{groupScope}/{isSecurityGroup}")]
        AdGroup CreateGroup(string apikey, string name, string location, string description, string groupScope, string isSecurityGroup);

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
        [OperationContract(Name = "CreateUser")]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/user/create/{firstName}/{lastName}/{samAccountName}/{location}")]
        AdUser CreateUser(string apikey, string firstName, string lastName, string samAccountName, string location);

        /// <summary>
        /// DeleteComputer
        /// 
        /// Deletes the specified computer.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier of the computer.</param>
        /// <returns>True if the computer was deleted, false otherwise.</returns>
        [OperationContract(Name = "DeleteComputer")]
        [WebInvoke(Method = "DELETE", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/computer/delete/{name}")]
        Boolean DeleteComputer(string apikey, string name);

        /// <summary>
        /// DeleteGroup
        /// 
        /// Deletes the specified group.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier of the group.</param>
        /// <returns>True if the group was deleted, false otherwise.</returns>
        [OperationContract(Name = "DeleteGroup")]
        [WebInvoke(Method = "DELETE", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/group/delete/{name}")]
        Boolean DeleteGroup(string apikey, string name);

        /// <summary>
        /// DeleteUser
        /// 
        /// Deletes the specified user.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier of the user.</param>
        /// <returns>True if the user was deleted, false otherwise.</returns>
        [OperationContract(Name = "DeleteUser")]
        [WebInvoke(Method = "DELETE", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/user/delete/{name}")]
        Boolean DeleteUser(string apikey, string name);

        /// <summary>
        /// DisableUser
        /// 
        /// Disables the specified user.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier of the user.</param>
        /// <returns>True if the user was deleted, false otherwise.</returns>
        [OperationContract(Name = "DisableUser")]
        [WebInvoke(Method = "PUT", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/user/disable/{name}")]
        Boolean DisableUser(string apikey, string name);

        /// <summary>
        /// EnableUser
        /// 
        /// Sets the enabled flag on the specified user to true.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier for the user.</param>
        /// <returns>True if enabled, false otherwise.</returns>
        [OperationContract(Name = "EnableUser")]
        [WebInvoke(Method = "PUT", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/user/enable/{name}")]
        Boolean EnableUser(string apikey, string name);

        /// <summary>
        /// Exists
        /// 
        /// Checks to see if the specified item exists. The item can be a user, group, computer, or OU.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="item">The item to test. This can be a SamAccountName, UserPrincipalName, or DistinguishedName.</param>
        /// <returns>True if the item exists, false otherwise.</returns>
        [OperationContract(Name = "Exists")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/exists/{item}")]
        Boolean Exists(string apikey, string item);

        /// <summary>
        /// ExpirePassword
        /// 
        /// Expires the password of the specified principal. The principal can be either a user or computer.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The name of the principal that should have its password expired.</param>
        /// <returns>True if the password was expired, false otherwise.</returns>
        [OperationContract(Name = "ExpirePassword")]
        [WebInvoke(Method = "PUT", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/expirepassword/{name}")]
        Boolean ExpirePassword(string apikey, string name);

        /// <summary>
        /// GetComputer
        /// 
        /// Returns the attributes of the specified computer.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">A unique identifier for the computer, either a Name, SamAccountName, DistinguishedName, or UserPrincipalName.</param>
        /// <returns>An AdComputer object.</returns>
        [OperationContract(Name = "GetComputer")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/computer/get/{name}")]
        AdComputer GetComputer(string apikey, string name);

        /// <summary>
        /// GetComputersInGroup
        /// 
        /// Returns a list of AdComputer objects that are members of the specified group.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="group">The unique identifier of the group.</param>
        /// <returns>A list of AdComputer objects.</returns>
        [OperationContract(Name = "GetComputersInGroup")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/group/get/computers/{group}")]
        List<AdComputer> GetComputersInGroup(string apikey, string group);

        /// <summary>
        /// GetCurrentDomain
        /// 
        /// Returns the Full domain name of the current context, i.e., the context of the accout the code is running under.
        /// In IIS this would be the Application Pool identity.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <returns>A string that is the full domain name of the current PrincipalContext.</returns>
        [OperationContract(Name = "GetCurrentDomain")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/domain/get/current")]
        string GetCurrentDomain(string apikey);

        /// <summary>
        /// GetDistinctUserProperty
        /// 
        /// Returns the distinct values of the specified property found on any user in the specified OU.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="property">Distinct values of this property will be returned.</param>
        /// <param name="location">The distinguished name of the OU to look in.</param>
        /// <returns>A list of strings that are sorted distinct values of the specified property.</returns>
        [OperationContract(Name = "GetDistinctUserProperty")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/user/property/get/distinct/{property}/{location}")]
        List<string> GetDistinctUserProperty(string apikey, string property, string location);

        /// <summary>
        /// GetDistinguishedName
        /// 
        /// Returns the DistinguishedName associated with the specified Principal, which is a user, computer, group, or OU.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier for the Principal.</param>
        /// <returns>A string that is the distinguishedName of the specified Principal.</returns>
        [OperationContract(Name = "GetDistinguishedName")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/get/distinguishedname/{name}")]
        string GetDistinguishedName(string apikey, string name);

        /// <summary>
        /// GetDomainContollers
        /// 
        /// Returns a list of all domain controllers in the current domain.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <returns>
        /// A list of strings that are the fully qualified domain names of all the domain controllers in the current domain.
        /// </returns>
        [OperationContract(Name = "GetDomainControllers")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/domain/get/domaincontrollers")]
        List<string> GetDomainControllers(string apikey);

        /// <summary>
        /// GetForestDomains
        /// 
        /// Returns a list of all the domains in the Forest.
        /// </summary>
        /// <param name="apikey">The apikey used to authorize the request.</param>
        /// <returns>A list of strings that are the domains in the Forest.</returns>
        [OperationContract(Name = "GetForestDomains")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/forest/get/domains")]
        List<string> GetForestDomains(string apikey);

        /// <summary>
        /// GetFullDomainName
        /// 
        /// Returns the full domain name based on the friendly name. For example, in the my.mega.corp domain passing in
        /// 'my' would return 'my.mega.corp'.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="domain">The friendly domain name. In my.domain it would be 'my'.</param>
        /// <returns>A string that is the full domain name of the specified friendly name.</returns>
        [OperationContract(Name = "GetFullDomainName")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/domain/get/fullname/{domain}")]
        string GetFullDomainName(string apikey, string domain);

        /// <summary>
        /// GetGlobalCatalogServers
        /// 
        /// Returns a list of all domain controllers that are Global Catalog servers.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <returns>
        /// A list of strings that are the fully qualified domain names of the Global Catalog servers in the current domain.
        /// </returns>
        [OperationContract(Name = "GetGlobalCatalogServers")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/domain/get/globalcatalogservers")]
        List<string> GetGlobalCatalogServers(string apikey);

        /// <summary>
        /// GetGroup
        /// 
        /// Returns the attributes and values of the specified group.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">A unique identifier for the group.</param>
        /// <returns>An AdGroup object.</returns>
        [OperationContract(Name = "GetGroup")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/group/get/{name}")]
        AdGroup GetGroup(string apikey, string name);

        /// <summary>
        /// GetGroups
        /// 
        /// Returns a list of groups in the specified OU.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="location">The distinguished name of the OU in which to look for groups.</param>
        /// <returns>A list of AdGroup objects.</returns>
        [OperationContract(Name = "GetGroups")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/group/get/location/{location}")]
        List<AdGroup> GetGroups(string apikey, string location);

        /// <summary>
        /// GetGroupsInGroup
        /// 
        /// Returns a list of groups that are members of the specified group.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="group">The unique identifier of the group.</param>
        /// <returns>A list of AdGroup objects.</returns>
        [OperationContract(Name = "GetGroupsInGroup")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/group/get/groups/{group}")]
        List<AdGroup> GetGroupsInGroup(string apikey, string group);

        /// <summary>
        /// GetGroupMembership
        /// 
        /// Returns a list of strings that are the SamAccountNames of the groups the specified Principal is a member of.
        /// The Principal is a user, group, or computer.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifer of the principal.</param>
        /// <returns>A list of strings that are SamAccountNames of the groups the specified Principal is a member of.</returns>
        [OperationContract(Name = "GetGroupMembership")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/get/membership/{name}")]
        List<string> GetGroupMembership(string apikey, string name);

        /// <summary>
        /// GetItems
        /// 
        /// Returns a list of distinguished names that are children of the specified OU.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="location">The distinguished name of the specified OU.</param>
        /// <returns>A list of strings that are the distinguished names of the children of the specified OU.</returns>
        [OperationContract(Name = "GetItems")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/get/items/{location}")]
        List<string> GetItems(string apikey, string location);

        /// <summary>
        /// GetMembers
        /// 
        /// Returns the members of the specified group. Returned items may be a user, computer, or group.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="group">The unique identifier for the group.</param>
        /// <returns>A list of AdMember objects.</returns>
        [OperationContract(Name = "GetMembers")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/group/get/members/{group}")]
        List<AdMember> GetMembers(string apikey, string group);

        /// <summary>
        /// GetProperty
        /// 
        /// Returns the value of the specified property of the specified Principal.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier of the Principal. The principal can be a user, group, or computer.</param>
        /// <param name="property">The name of the property.</param>
        /// <returns>A string representing the value of the specified property of the specified Principal.</returns>
        [OperationContract(Name = "GetProperty")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/get/property/{name}/{property}")]
        string GetProperty(string apikey, string name, string property);

        /// <summary>
        /// GetRandomPassword
        /// 
        /// Returns a password that is 12 characters long and has at least 3 special chars.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <returns>A string that is a random password 12 characters long.</returns>
        [OperationContract(Name = "GetRandomPassword")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/get/randompassword")]
        string GetRandomPassword(string apikey);

        /// <summary>
        /// GetRootContainer
        /// 
        /// Returns the full domain name of the current domain reformatted as the root ldap level. For example, if the 
        /// current domain is my.mega.corp, DC=my,DC=mega,DC=corp is returned.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <returns>A string that is the top level ldap path.</returns>
        [OperationContract(Name = "GetRootContainer")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/domain/get/rootcontainer")]
        string GetRootContainer(string apikey);

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
        [OperationContract(Name = "GetSamAccountName")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/get/samaccountname/{name}")]
        string GetSamAccountName(string apikey, string name);

        /// <summary>
        /// GetUsedAttributes
        /// 
        /// Returns the attributes names and values of any attributes that are not empty. Some attributes are represented
        /// by the object's tostring value because microsoft can't use a regular DateTime like everyone else.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier of the Principal whose atttributes should be returned.</param>
        /// <returns>A dictionary where the keys are the attribute names and the values are the attribute values.</returns>
        [OperationContract(Name = "GetUsedAttributes")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/get/attributes/{name}")]
        SortedDictionary<string, string> GetUsedAttributes(string apikey, string name);

        /// <summary>
        /// GetUser
        /// 
        /// Returns the property names and values of the specified UserPrincipal.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="username">The unique identifier of the UserPrincipal.</param>
        /// <returns>An AdUser object.</returns>
        [OperationContract(Name = "GetUser")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/user/get/{username}")]
        AdUser GetUser(string apikey, string username);

        /// <summary>
        /// GetUserByEmail
        /// 
        /// Returns an AdUser object associated with the specified email account. This method
        /// only returns users from OUs that have ",Users," in their distinguished names.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="email">The email address that identifies a user account.</param>
        /// <returns>An AdUser object with an email address that is the first match to the specified email address.</returns>
        [OperationContract(Name = "GetUserByEmail")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/user/get/email/{email}")]
        AdUser GetUserByEmail(string apikey, string email);

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
        [OperationContract(Name = "GetUsers")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/user/get/location/{location}")]
        List<AdUser> GetUsers(string apikey, string location);

        /// <summary>
        /// GetUsersInGroup
        /// 
        /// Returns a list of users that are members of the specified group.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="group"></param>
        /// <returns>A list of AdUser objects.</returns>
        [OperationContract(Name = "GetUsersInGroup")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/group/get/users/{group}")]
        List<AdUser> GetUsersInGroup(string apikey, string group);

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
        [OperationContract(Name = "GetUserSelect")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/user/get/select/{name}/{select}")]
        List<SortedDictionary<string, string>> GetUserSelect(string apikey, string name, string select);

        /// <summary>
        /// IsLockedOut
        /// 
        /// Returns true if the specified Principal, a user or computer, is locked out.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier of the user.</param>
        /// <returns>True if the user account is locked, false otherwise.</returns>
        [OperationContract(Name = "IsLocked")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/islocked/{name}")]
        Boolean IsLocked(string apikey, string name);

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
        [OperationContract(Name = "IsMember")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/ismember/{principal}/{group}")]
        Boolean IsMember(string apikey, string principal, string group);

        /// <summary>
        /// Move
        /// 
        /// Moves the specified Principal to the specified OU.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifer of the Principal.</param>
        /// <param name="location">The path, i.e., distinguished name of the OU that the Principal should be moved to.</param>
        /// <returns>True if the principal was moved, false otherwise.</returns>
        [OperationContract(Name = "Move")]
        [WebInvoke(Method = "PUT", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/move/{name}/{location}")]
        Boolean Move(string apikey, string name, string location);

        /// <summary>
        /// RemoveMember
        /// 
        /// Removes the specified member from the specified group.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="member">The member to remove.</param>
        /// <param name="group">The group from which the member should be removed.</param>
        /// <returns>A boolean, true if the member was removed from the group, false otherwise.</returns>
        [OperationContract(Name = "RemoveMember")]
        [WebInvoke(Method = "DELETE", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/group/remove/{member}/{group}")]
        Boolean RemoveMember(string apikey, string member, string group);

        /// <summary>
        /// SetManager
        /// 
        /// Sets the Manager property of the specified user to the distinguished name of the specified manager.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="user">The unique identifier for the user.</param>
        /// <param name="manager">The unique identifier for the manager</param>
        /// <returns>A Boolean, true if the Manager attribute was updated, false otherwise.</returns>
        [OperationContract(Name = "SetManager")]
        [WebInvoke(Method = "PUT", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/user/set/manager/{user}/{manager}")]
        Boolean SetManager(string apikey, string user, string manager);

        /// <summary>
        /// SetPassword
        /// 
        /// Sets the password of the specified user to the specified password.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier of the user.</param>
        /// <param name="password">The new password for the user.</param>
        /// <returns>True if success, false otherwise.</returns>
        [OperationContract(Name = "SetPassword")]
        [WebInvoke(Method = "PUT", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/user/set/password/{name}/{password}")]
        Boolean SetPassword(string apikey, string name, string password);

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
        [OperationContract(Name = "SetProperty")]
        [WebInvoke(Method = "PUT", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/set/property/{name}/{property}/{value}")]
        Boolean SetProperty(string apikey, string name, string property, string value);

        /// <summary>
        /// UnlockAccount
        /// 
        /// Unlocks the specified user or computer account.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="name">The unique identifier for the computer or user.</param>
        /// <returns>True if success, false otherwise.</returns>
        [OperationContract(Name = "Unlock")]
        [WebInvoke(Method = "PUT", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/unlock/{name}")]
        Boolean Unlock(string apikey, string name);

        /// <summary>
        /// ValidateCredentials
        /// 
        /// Tests that the specified password is valid for the specified user.
        /// </summary>
        /// <param name="apikey">The key used to authorize the request.</param>
        /// <param name="username">The SamAccountName/logon name/username of the user.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>True if the credentials are valid, false otherwise.</returns>
        [OperationContract(Name = "ValidateCredentials")]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "{apikey}/user/validatecredentials/{username}/{password}")]
        Boolean ValidateCredentials(string apikey, string username, string password);

    }

}
