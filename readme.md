# Active Directory API RESTful JSON Service

version 1.1.0

Provides an easy to use interface for managing Active Directory users, groups, and computers.

-----------------

Contents:

* [Security](#security)
* [Data Format](#data-format)
* [Methods](#methods)
  * [AddMember](#addmember)
  * [CopyUser](#copyuser)
  * [CreateComputer](#createcomputer)
  * [CreateGroup](#creategroup)
  * [CreateUser](#createuser)
  * [DeleteComputer](#deletecomputer)
  * [DeleteGroup](#deletegroup)
  * [DeleteUser](#deleteuser)
  * [DisableUser](#disableuser)
  * [EnableUser](#enableuser)
  * [Exists](#exists)
  * [ExpirePassword](#expirepassword)
  * [GetComputer](#getcomputer)
  * [GetComputersInGroup](#getcomputersingroup)
  * [GetCurrentDomain](#getcurrentdomain)
  * [GetDistinctUserProperty](#getdistinctuserproperty)
  * [GetDistinguishedName](#getdistinguishedname)
  * [GetDomainControllers](#getdomaincontrollers)
  * [GetForestDomains](#getforestdomains)
  * [GetFullDomainName](#getfulldomainname)
  * [GetGlobalCatalogServers](#getglobalcatalogservers)
  * [GetGroup](#getgroup)
  * [GetGroups](#getgroups)
  * [GetGroupsInGroup](#getgroupsingroup)
  * [GetGroupMembership](#getgroupmembership)
  * [GetItems](#getitems)
  * [GetMembers](#getmembers)
  * [GetProperty](#getproperty)
  * [GetRandomPassword](#getrandompassword)
  * [GetRootContainer](#getrootcontainer)
  * [GetSamAccountName](#getsamaccountname)
  * [GetUsedAttributes](#getusedattributes)
  * [GetUser](#getuser)
  * [GetUserByEmail](#getuserbyemail)
  * [GetUsers](#getusers)
  * [GetUsersInGroup](#getusersingroup)
  * [GetUserSelect](#getuserselect)
  * [IsLocked](#islocked)
  * [IsMember](#ismember)
  * [Move](#move)
  * [RemoveMember](#removemember)
  * [SetManager](#setmanager)
  * [SetPassword](#setpassword)
  * [SetProperty](#setproperty)
  * [Unlock](#unlock)
  * [ValidateCredentials](#validatecredentials)

-----------------

## Security

This API should only be accessed via SSL and limited to specific IP addresses via firewall and IIS IP and Domain Restrictions. Careful consideration should be taken before implementing this.

### API Keys

Access is granted via API keys specified in the Web.config file AppSettings section.

The key is the GUID and the value is either:

1. An asterix that denotes all access.
2. A case insensitive comma separated list of methods the key is allowed to access.
3. A case insensitive comma separated list of http methods the key is allowed to use. Valid entries are get, put, post, delete.
4. A combination of 2 and 3, whichever matches first.

Items are matched in the following order, first match returns true:

1. Asterix (allaccess)
2. Method name
3. HttpMethodName

### Examples

    <!-- Test key: All access -->
    <add key="191d8ed8-c979-4586-8e09-9fa18b5e4c99" value="*"/>
    
    <!-- Test key: httpMethod limited -->
    <add key="bf0ede69-94ac-471a-ba6c-d9059a33efb0" value="get, put"/>
    
    <!-- Test key: Method Limited -->
    <add key="a3fff28f-777c-471b-a916-23ae96c3ae21" value="GetUser, GetGroup, GetGroupMembership, CreateUser, Exists, Unlock"/>
    
    <!-- Test key: Combined Access -->
    <add key="a3fff28f-777c-471b-a916-23ae96c3ae21" value="CreateUser, Unlock, GET"/>

### Protected Objects

Also in the AppSettings section of the Web.config file is a key named *protected_objects*. The value is a comma separated list of users, groups, and/or computers that should not
be altered by any PUT or DELETE operations. The values in the list must be SamAccountNames.

### Examples

    <add key="protected_objects" value="Domain Admins, Enterprise Admins, Schema Admins"/>

[top](#active-directory-api-restful-json-service)

-----------------

# Data Format

## AdUser

* string AccountExpirationData
* string AccountLockoutTime
* Boolean AllowReversiblePasswordEncryption
* int BadLogonCount
* string Company
* Boolean DelegationPermitted
* string Department
* string Description
* string DisplayName
* string DistinguishedName
* string EmailAddress
* string EmployeeId
* Boolean? Enabled
* string GivenName
* string Guid
* string HomeDirectory
* string HomeDrive
* string LastBadPasswordAttempt
* string LastLogon
* string LastPasswordSet
* string Manager
* List<string> MemberOf
* string MiddleName
* string Mobile
* string Name
* Boolean PasswordNeverExpires
* Boolean PasswordNotRequired
* string ProfilePath
* string SamAccountName
* string ScriptPath
* string Sid
* Boolean SmartCardLogonRequired
* string Surname
* Boolean UserCannotChangePassword
* string UserPrincipalName
* string VoiceTelephoneNumber

## AdComputer

* string AccountExpirationDate
* string AccountLockouttime
* int BadLogonCount
* Boolean DelegationPermitted
* string Description
* string DisplayName
* string DistinguishedName
* Boolean? Enabled
* string Guid
* string HomeDirectory
* string HomeDrive
* string LastBadPasswordAttempt
* string Name
* Boolean PasswordNeverExpires
* Boolean PasswordNotRequired
* string SamAccountName
* string ScriptPath
* List<string> ServicePrincipalNames
* string Sid
* Boolean SmartcardLogonRequired
* Boolean UserCannotChangePassword
* string UserPrincipalName
        
## AdGroup

* string Description
* string DisplayName
* string DistinguishedName
* string GroupScope
* string Guid
* Boolean? IsSecurityGroup
* List<AdMember> Members
* string Name
* string SamAccountName
* string Sid
* string UserPrincipalName
        
## AdMember

* string Description
* string DisplayName
* string DistinguishedName
* string Guid
* string Name
* string SamAccountName
* string Sid
* string UserPrincipalName

[top](#active-directory-api-restful-json-service)
        
-----------------

# Methods

## AddMember

Adds the specified principal to the specified group.

HttpMethod: PUT

### Parameters

* apikey - The key used to authenticate the request.
* member - A unique identifier for a user or group, for example, SamAccountName.
* group - A unique identifier for a group, for example, SamAccountName.

### Returns

A Boolean. True if the member was added to the group, false otherwise. This can return
false if the member or group does not exist.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseUrl}/{apikey}/group/add/{member}/{group}
    
    // Add user with SamAccountName smithj to the AwesomeUsers group
    https://adapi.my.domain/da1f432a-6c04-4992-b558-e6fa06001dd2/group/add/smithj/awesomeusers

[top](#active-directory-api-restful-json-service)

-----------------
    
## CopyUser

Copies a user account to the specified location. A random password will be assigned to the user
and the account will be disabled. Before the account can be used it will need to be
unlocked and have its password reset.

The following attributes are copied from the template:
description, co, company, countryCode, department, l, physicalDeliveryOfficeName, postalCode,
profilePath (modified for the new user), st, streetAddress.

It also copies the group membership of the template.

HttpMethod: POST

### Parameters

* apikey - The key used to authorize the request.
* firstName - The givenName of the new user.
* lastName - The surName (sn) of the new user.
* samAccountName - The new logon name. This must be unique or this method will fail.
* location - The distinguishedName of the OU where the new user should be created.
* templateSamAccountName - The SamAccountName of the user account that should be copied.

### Returns

An AdUser object representing the new user.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/user/copy/{firstName}/{lastName}/{samAccountName}/{location}/{templateSamAccountName}
    
    // Copy the user hrUserTemplate as Jeff Smith to the UsersHr OU
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/user/copy/Jeff/Smith/smithj/ou=UsersHr,dc=my,dc=domain/hrusertemplate

[top](#active-directory-api-restful-json-service)
    
-----------------

## CreateComputer

Creates a computer with the specified name and description in the specified location.

HttpMethod: POST

### Parameters

* apikey - The key used to authorize the request.
* name - The name of the computer.
* location - The distinguished name of the OU to create the computer in.
* description - The value that will be assigned to the new computer's description property.

### Returns

The AdComputer object of the new computer.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseUrl}/{apikey}/computer/create/{name}/{location}/{description}
    
    // Create a computer account named comp0069 in the ComputersHr OU.
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/computer/create/comp0069/ou=computershr,dc=my,dc=domain/Desktop - Smith, Jeff

[top](#active-directory-api-restful-json-service)

-----------------

## CreateGroup

Creates a new group in the specified location.

HttpMethod: POST

### Parameters

* apikey - The key used to authorize the request.
* name - The name of the group will be used for the name, display name, and SamAccountName.
* location - The distinguished name of the OU in which the group should be created.
* description - The description of the group.
* groupScope - The GroupScope. One of Global, Local, or Univeral.
* isSecurityGroup - 

### Returns

An AdGroup object representing the new group.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/group/create/{name}/{location}/{description}/{groupScope}/{isSecurityGroup}
    
    // Create a group called ACL_Personnel_MODIFY in the GroupsHr OU.
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/group/create/ACL_Personnel_MODIFY/ou=groupshr,dc=my,dc=domain/Contains users with modify permissions to HR resources/local/true

[top](#active-directory-api-restful-json-service)

-----------------

## CreateUser

Creates a new user account in the specified location. The new account will be disabled.

HttpMethod: POST

### Parameters

* apikey - The key used to authorize the request.
* firstName - The givenName of the new user.
* lastName - The surname of the new user.
* samAccountName - the SamAccountName of the new user.
* location - The distinguished name of the OU in which the new user should be created.

### Returns

An AdUser account representing the new user.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/user/create/{firstName}/{lastName}/{samAccountName}/{location}
    
    // Create a user account for Jeff Smith in the UsersHr OU.
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/user/create/Jeff/Smith/smithj/ou=usershr,dc=my,dc=domain

[top](#active-directory-api-restful-json-service)

-----------------

## DeleteComputer

Deletes the specified computer.

HttpMethod: DELETE

### Parameters

* apikey - The key used to authorize the request.
* name - The unique identifier of the computer.

### Returns

True if the computer was deleted, false otherwise.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/computer/delete/{name}
    
    // Delete the computer named thx1138
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/computer/delete/thx1138

[top](#active-directory-api-restful-json-service)

-----------------

## DeleteGroup

Deletes the specified group.

HttpMethod: DELETE

### Parameters

* apikey - The key used to authorize the request.
* name - The unique identifier of the group.

### Returns

True if the group was deleted, false otherwise.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/group/delete/{name}
    
    // Delete the group named MegaCorpGuests
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/group/delete/megacorpguests

[top](#active-directory-api-restful-json-service)

-----------------

## DeleteUser

Deletes the specified user.

HttpMethod: DELETE

### Parameters

* apikey - The key used to authorize the request.
* name - The unique identifier of the user.

### Returns

True if the user was deleted, false otherwise.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/user/delete/{name}
    
    // Delete the user with SamAccountName smithj
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/user/delete/smithj

[top](#active-directory-api-restful-json-service)

-----------------

## DisableUser

Disables the specified user.

HttpMethod: PUT

### Parameters

* apikey - The key used to authorize the request.
* name - The unique identifier of the user.

### Returns

True if the user was deleted, false otherwise.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/user/disable/{name}
    
    // Disable user account with SamAccountName smithj
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/user/disable/smithj

[top](#active-directory-api-restful-json-service)

-----------------

## EnableUser

Sets the enabled flag on the specified user to true.

HttpMethod: PUT

### Parameters

* apikey - The key used to authorize the request.
* name - The unique identifier for the user.

### Returns

True if enabled, false otherwise.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/user/enable/{name}
    
    // Enable user account with SamAccountName smithj
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/user/enable/smithj

[top](#active-directory-api-restful-json-service)

-----------------

## Exists

Checks to see if the specified item exists. The item can be a user, group, computer, or OU.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.
* item - The item to test. This can be a SamAccountName, UserPrincipalName, or DistinguishedName.

### Returns

True if the item exists, false otherwise.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/exists/{item}
    
    // Test to see if user with SamAccountName smithj exists
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/exists/smithj
    
    // Test to see if group with SamAccountName MegaCorpGuests exists
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/exists/megacorpguests
    
    // Test to see if OU UsersHr exists
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/exists/ou=usershr,dc=my,dc=domain

[top](#active-directory-api-restful-json-service)
    
-----------------

## ExpirePassword

Expires the password of the specified principal. The principal can be either a user or computer.

HttpMethod: PUT

### Parameters

* apikey - The key used to authorize the request.
* name - The name of the principal that should have its password expired.

### Returns

True if the password was expired, false otherwise.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/expirepassword/{name}
    
    // Expire the password of user with SamAccountName smithj
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/expirepassword/smithj
    
    // Expire the password of computer thx1138
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/expirepassword/thx1138
    
[top](#active-directory-api-restful-json-service)

-----------------

## GetComputer

Returns the attributes of the specified computer.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.
* name - A unique identifier for the computer, either a Name, SamAccountName, DistinguishedName, or UserPrincipalName.

### Returns

Ad AdComputer object.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/computer/get/{name}
    
    // Get the details of the computer with SamAccountName thx1138
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/computer/get/thx1138

[top](#active-directory-api-restful-json-service)

-----------------

## GetComputersInGroup

Returns a list of AdComputer objects that are members of the specified group.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.
* group - The unique identifier of the group.

### Returns

A list of AdComputer objects.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/group/getcomputers/{group}
    
    // Get only computers that are members of the group ComputersHr
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/group/get/computers/computershr

[top](#active-directory-api-restful-json-service)

-----------------

GetCurrentDomain

Returns the Full domain name of the current context, i.e., the context of the accout the code is running under.
In IIS this would be the Application Pool identity.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.

### Returns

A string that is the full domain name of the current PrincipalContext.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/domain/get/current
    
    // Get the full domain name of the current principal context's, i.e., the account the code is running as,
    // domain. In IIS it would be the Application Pool identity.
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/domain/get/current

[top](#active-directory-api-restful-json-service)

-----------------

GetDistinctUserProperty

Returns the distinct values of the specified property found on any user in the specified OU.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.
* property - Distinct values of this property will be returned.
* location - The distinguished name of the OU to look in.

### Returns

A list of strings that are sorted distinct values of the specified property.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/user/property/get/distinct/{property}/{location}
    
    // Get a list of distinct departments in the UsersHr OU.
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/user/property/get/distinct/department/ou=usershr,dc=my,dc=domain

[top](#active-directory-api-restful-json-service)

-----------------

## GetDistinguishedName

Returns the DistinguishedName associated with the specified Principal, which is a user, computer, group, or OU.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.
* name - The unique identifier for the Principal.

### Returns

A string that is the distinguishedName of the specified Principal.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/get/distinguishedname/{name}
    
    // Get the distinguished name of the user with SamAccountName smithj
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/get/distinguishedname/smithj

[top](#active-directory-api-restful-json-service)

-----------------

## GetDomainContollers

Returns a list of all domain controllers in the current domain.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.

### Returns

A list of strings that are the fully qualified domain names of all the domain controllers in the current domain.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/domain/get/domaincontrollers
    
    // Get a list of all domain controllers in the current domain
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/domain/get/domaincontrollers

[top](#active-directory-api-restful-json-service)

-----------------

## GetForestDomains

Returns a list of all the domains in the Forest.

HttpMethod: GET

### Parameters

* apikey - The apikey used to authorize the request.

### Returns

A list of strings that are the domains in the Forest.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/forest/get/domains
    
    // Get a list of all domains in the forest
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/forest/get/domains

[top](#active-directory-api-restful-json-service)

-----------------

## GetFullDomainName

Returns the full domain name based on the friendly name. For example, in the my.mega.corp domain passing in 'my' would return 'my.mega.corp'.

HttpMethod: GET

### Parameters

* apikey - The apikey used to authorize the request.
* domain - The friendly domain name. In my.domain it would be 'my'.

### Returns

The full domain name of the specified friendly domain name.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/domain/get/fullname/{domain}
    
    // Get the full domain name of the super.mega.corp domain
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/domain/get/fullname/super

[top](#active-directory-api-restful-json-service)

-----------------

## GetGlobalCatalogServers

Returns a list of all domain controllers that are Global Catalog servers.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.

### Returns

A list of strings that are the fully qualified domain names of the Global Catalog servers in the current domain.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/domain/get/globalcatalogservers
    
    // Get a list of domain controllers that are global catalog servers.
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/domain/get/globalcatalogservers
    
[top](#active-directory-api-restful-json-service)

-----------------

## GetGroup

Returns the attributes and values of the specified group.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.
* name - A unique identifier for the group.

### Returns

An AdGroup object.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/group/get/{name}
    
    // Get the details of the UsersHr group
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/group/get/usershr

[top](#active-directory-api-restful-json-service)

-----------------

## GetGroups

Returns a list of groups in the specified OU.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.
* location - The distinguished name of the OU in which to look for groups.

### Returns

A list of AdGroup objects.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/group/get/location/{location}
    
    // Get a list of groups in the UsersGroups OU
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/group/get/location/ou=usersgroups,dc=my,dc=domain

[top](#active-directory-api-restful-json-service)

-----------------

## GetGroupsInGroup

Returns a list of groups that are members of the specified group.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.
* group - The unique identifier of the group.

### Returns

A list of AdGroup objects.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/group/get/groups/{group}
    
    // Get only the groups that are members of the GPO_Policy group
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/group/get/groups/gpo_policy

[top](#active-directory-api-restful-json-service)

-----------------

## GetGroupMembership

Returns a list of strings that are the SamAccountNames of the groups the specified Principal is a member of.
The Principal is a user, group, or computer.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.
* name - The unique identifer of the principal.

### Returns

A list of strings that are SamAccountNames of the groups the specified Principal is a member of.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/get/membership/{name}
    
    // Get the SamAccountNames of the groups the user with the SamAccountName smithj is a member of
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/get/membership/smithj

[top](#active-directory-api-restful-json-service)

-----------------

## GetItems

Returns a list of distinguished names that are children of the specified OU.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.
* location - The distinguished name of the specified OU.

### Returns

A list of strings that are the distinguished names of the children of the specified OU.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/get/items/{location}
    
    // Get the distinguished names of all items in the HumanResources OU
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/get/items/ou=humanresources,dc=my,dc=domain

[top](#active-directory-api-restful-json-service)

-----------------

## GetMembers

Returns the members of the specified group. Returned items may be a user, computer, or group.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.
* group - The unique identifier for the group.

### Returns

A list of AdMember objects.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/group/get/members/{group}
    
    // Get all members, i.e., users, computers, and groups, in the GPO_Policy group.
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/group/get/members/gpo_policy

[top](#active-directory-api-restful-json-service)

-----------------

## GetProperty

Returns the value of the specified property of the specified Principal.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.
* name - The unique identifier of the Principal. The principal can be a user, group, or computer.
* property - The name of the property.

### Returns

A string representing the value of the specified property of the specified Principal.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/get/property/{name}/{property}
    
    // Get the value of the manager property of the user with SamAccountName smithj
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/get/property/smithj/manager

[top](#active-directory-api-restful-json-service)

-----------------

## GetRandomPassword

Returns a password that is 12 characters long and has at least 3 special chars.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.

### Returns

A string that is a random password 12 characters long.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/get/randompassword
    
    // Get a random password
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/get/randompassword

[top](#active-directory-api-restful-json-service)

-----------------

## GetRootContainer

Returns the full domain name of the current domain reformatted as the root ldap level. For example, if the 
current domain is my.mega.corp, DC=my,DC=mega,DC=corp is returned.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.

### Returns

A string that is the top level ldap path.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/domain/get/rootcontainer
    
    // Get the root container
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/domain/get/rootcontainer

[top](#active-directory-api-restful-json-service)

-----------------

## GetSamAccountName

Returns the SamAccountName of the Principal identified by the specified name which can be a distinguished name, 
UserPrincipalName, or SamAccountName. The Princiapl must be either a user, computer, or group. If passing in a
distinguished name keep in mind that some distinguished  names can't be passed in a URL, for example, if a user's 
name is in lastname, firstname format the distinguished name will have a backslash to escape the comma.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.
* name - The unique identifier of the Principal whose SamAccountName should be returned.

### Returns

A string that is the SamAccountName of the Principal specified by the DistinguishedName.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/get/samaccountname/{name}
    
    // Get the sam account name of user with UserPrincipalName smithj@my.domain
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/get/samaccountname/smithj@my.domain

[top](#active-directory-api-restful-json-service)

-----------------

## GetUsedAttributes

Returns the attributes names and values of any attributes that are not empty. Some attributes are represented
by the object's tostring value because microsoft can't use a regular DateTime like everyone else. This is 
rarely used. GetUser is usually a better choice, but this is here if you need it.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.
* name - The unique identifier of the Principal whose atttributes should be returned.

### Returns

A dictionary where the keys are the attribute names and the values are the attribute values.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/get/attributes/{name}
    
    // Get all the attributes with values for user with SamAccountName smithj
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/get/attributes/smithj

[top](#active-directory-api-restful-json-service)

-----------------

## GetUser

Returns the property names and values of the specified UserPrincipal.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.
* username - The unique identifier of the UserPrincipal.

### Returns

An AdUser object.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/user/get/{username}
    
    // Get the user with SamAccountName smithj
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/user/get/smithj

[top](#active-directory-api-restful-json-service)

-----------------

## GetUserByEmail

Returns an AdUser object associated with the specified email account. This method
only returns users from OUs that have ",Users," in their distinguished names.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.
* email - The email address that identifies a user account.

### Returns

An AdUser object with an email address that is the first match to the specified email address.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URl is in the form {baseurl}/{apikey}/user/get/email/{email}
    
    // Get the details of the user with email address smithj@my.domain
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/user/get/email/smithj@my.domain

[top](#active-directory-api-restful-json-service)

-----------------

## GetUsers

Returns all users in the specified OU.

### Parameters

* apikey - The key used to authorize the request.
* location - The path, i.e., the distinguished name, of the OU from in which the user accounts should exist.

HttpMethod: GET

### Returns

A list of AdUser objects.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/user/get/location/{location}
    
    // Get a list of users in the UsersHr OU
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/user/get/location/ou=usershr,dc=my,dc=domain

[top](#active-directory-api-restful-json-service)

-----------------

## GetUsersInGroup

Returns a list of users that are members of the specified group.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.
* group - 

### Returns

A list of AdUser objects.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URl is in the form {baseurl}/{apikey}/group/get/users/{group}
    
    // Get only the users that are members of the GPO_Policy group
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/group/get/users/gpo_policy

[top](#active-directory-api-restful-json-service)

-----------------

## GetUserSelect

Returns the specified properties of the specified user. The select parameter is a dot-delimited list
of properties, for example, to return just the SamAccountName, display name, and description the value
of the select parameter would be, "samaccountname.displayname.description".

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.
* name - The unique identifier of the user.
* select - A dot-delimited list of properties that should be returned.

### Returns

A list of dictionaries where the keys are the names of the properties and the values are the property values.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/user/get/select/{name}/{select}
    
    // Get the display name, department, and email address of user with SamAccountName smithj 
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/user/get/select/smithj/displayname.department.mail

[top](#active-directory-api-restful-json-service)

-----------------

## IsLocked

Returns true if the specified Principal, a user or computer, is locked out.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.
* name - The unique identifier of the user.

### Returns

True if the user account is locked, false otherwise.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/islocked/{name}
    
    // Test if the smithj user account is locked
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/islocked/smithj

[top](#active-directory-api-restful-json-service)

-----------------

## IsMember

Returns true if the specified principal, i.e., user, group, or computer, is a member of the specified group.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.
* principal - The unique identifier that corresponds to the Principal.
* group - The unique identifier that corresponds to the group.

### Returns

True if the principal is a member of the group, false otherwise.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/ismember/{principal}/{group}
    
    // Test if the user with SamAccountName smithj is a member of the UsersHr group
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/ismember/smithj/usershr

[top](#active-directory-api-restful-json-service)

-----------------

## Move

Moves the specified Principal to the specified OU.

HttpMethod: PUT

### Parameters

* apikey - The key used to authorize the request.
* name - The unique identifer of the Principal.
* location - The path, i.e., distinguished name of the OU that the Principal should be moved to.

### Returns

True if the principal was moved, false otherwise.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URl is in the form {baseurl}/{apikey}/move/{name}/{location}
    
    // Move the user with SamAccountName smithj to the UsersManagement OU
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/move/smithj/usersmanagement

[top](#active-directory-api-restful-json-service)

-----------------

## RemoveMember

Removes the specified member from the specified group.

HttpMethod: DELETE

### Parameters

* apikey - The key used to authorize the request.
* member - The member to remove.
* group - The group from which the member should be removed.

### Returns

A boolean, true if the member was removed from the group, false otherwise.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/group/remove/{member}/{group}
    
    // Remove user with SamAccountName smithj from the UsersHr group
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/group/remove/smithj/usershr

[top](#active-directory-api-restful-json-service)

-----------------

## SetManager

Sets the Manager property of the specified user to the distinguished name of the specified manager.

HttpMethod: PUT

### Parameters

* apikey - The key used to authorize the request.
* user - The unique identifier for the user.
* manager - The unique identifier for the manager

### Returns

A Boolean, true if the Manager attribute was updated, false otherwise.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/user/set/manager/{user}/{manager}
    
    // Set the manager property of user with SamAccountName smithj to user with SamAccountName mrmanager
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/user/set/manager/smithj/mrmanger

[top](#active-directory-api-restful-json-service)

-----------------

## SetPassword

Sets the password of the specified user to the specified password.

HttpMethod: PUT

### Parameters

* apikey - The key used to authorize the request.
* name - The unique identifier of the user.
* password - The new password for the user.

### Returns

True if success, false otherwise.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/user/set/password/{name}/{password}
    
    // Set the password of the user account with SamAccountName smithj to MyN3wP@$$w0rd!
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/user/set/password/smithj/MyN3wP@$$w0rd!

[top](#active-directory-api-restful-json-service)

-----------------

## SetProperty

Sets the specified property to the specified value on the specified Principal.

HttpMethod: PUT

### Parameters

* apikey - The key used to authorize the request.
* name - The unique identifier for the Principal.
* property - The name of the property.
* value - The value to assign to the property.

### Returns

True if success, false otherwise.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/set/property/{name}/{property}/{value}
    
    // Set the department property of the user with SamAccountName smithj to 'Human Resources'
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/set/property/smithj/department/Human Resources

[top](#active-directory-api-restful-json-service)

-----------------

## Unlock

Unlocks the specified user or computer account.

HttpMethod: PUT

### Parameters

* apikey - The key used to authorize the request.
* name - The unique identifier for the computer or user.

### Returns

True if success, false otherwise.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/unlock/{name}
    
    // Unlock the user account with SamAccountName smithj
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/unlock/smithj

[top](#active-directory-api-restful-json-service)

-----------------

## ValidateCredentials

Tests that the specified password is valid for the specified user and the account is not locked or disabled.

HttpMethod: GET

### Parameters

* apikey - The key used to authorize the request.
* username - The SamAccountName/logon name/username of the user.
* password - The user's password.

### Returns

True if the credentials are valid, false otherwise.

### Throws

* 401 Unauthorized if the apikey is invalid or does not have access to either the method or httpMethod.
* 500 InternalServerError if other errors, such as LDAP bind errors, occur.

### Examples

    // URL is in the form {baseurl}/{apikey}/user/validatecredentials/{username}/{password}
    
    // Test the credentials of user with SamAccountName smithj and password MyN3wP@$$w0rd
    https://adapi.my.domain/96e7171e-a339-4d9a-b823-d2b6c66de4a0/user/validatecredentials/smithj/MyN3wP@$$w0rd

[top](#active-directory-api-restful-json-service)

-----------------
