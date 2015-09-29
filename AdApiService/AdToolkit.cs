using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;

namespace AdApiService
{
    public class AdToolkit
    {
       
        /// <summary>
        /// Adds the specified member (group or user), to the specified group.
        /// </summary>
        /// <param name="member">The user or group to add to the specified group.</param>
        /// <param name="group">The group the member should be added to.</param>
        /// <returns>True if the member was added, false otherwise.</returns>
        public static Boolean AddMember(string member, string group)
        {
            Boolean done = false;
            GroupPrincipal g = GroupPrincipal.FindByIdentity(GetPrincipalContext(), group);
            Principal p = Principal.FindByIdentity(GetPrincipalContext(), member);
            if (!IsMember(member, group))
            {
                g.Members.Add(p);
                g.Save();
                done = IsMember(member, group);
            }
            return done;
        }

        /// <summary>
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
        /// <param name="firstName">The givenName of the new user.</param>
        /// <param name="lastName">The surName (sn) of the new user.</param>
        /// <param name="samAccountName">The new logon name. This must be unique or this method will fail.</param>
        /// <param name="location">The distinguishedName of the OU where the new user should be created.</param>
        /// <param name="templateSamAccountName"></param>
        /// <returns>The UserPrincipal object of the new user.</returns>
        public static UserPrincipal CopyUser(string firstName, string lastName, string samAccountName, string location, string templateSamAccountName)
        {
            // Get the template principal object and create a new user principal object
            UserPrincipal template = UserPrincipal.FindByIdentity(GetPrincipalContext(), templateSamAccountName);
            UserPrincipal newUser = new UserPrincipal(GetPrincipalContext(location), samAccountName, GetRandomPassword(), false);
            // create some attribute values for later
            string displayName = string.Format("{0}, {1}", lastName, firstName);
            string profilePath = GetProperty(template, "profilePath").Replace(templateSamAccountName, samAccountName);

            // some easy settings that are in the UserPrincipal object
            newUser.Description = template.Description;
            newUser.DisplayName = displayName;
            newUser.GivenName = firstName;
            newUser.Name = displayName;
            newUser.Surname = lastName;
            newUser.UserCannotChangePassword = false;
            newUser.UserPrincipalName = string.Format("{0}@{1}", samAccountName, GetCurrentDomain());
            newUser.Save();

            // some attributes must be set the old way
            SetProperty(newUser, "co", GetProperty(template, "co"));
            SetProperty(newUser, "company", GetProperty(template, "company"));
            SetProperty(newUser, "countryCode", "0");
            SetProperty(newUser, "department", GetProperty(template, "department"));
            SetProperty(newUser, "l", GetProperty(template, "l"));
            SetProperty(newUser, "physicalDeliveryOfficeName", GetProperty(template, "physicalDeliveryOfficeName"));
            SetProperty(newUser, "postalCode", GetProperty(template, "postalCode"));
            SetProperty(newUser, "profilePath", profilePath);
            SetProperty(newUser, "st", GetProperty(template, "st"));
            SetProperty(newUser, "streetAddress", GetProperty(template, "streetAddress"));

            // copy the group membership of the template
            foreach (GroupPrincipal group in template.GetGroups())
            {
                AddMember(samAccountName, group.SamAccountName);
            }

            return newUser;
        }

        /// <summary>
        /// Creates a computer with the specified name and description in the specified location.
        /// </summary>
        /// <param name="name">The name of the new computer.</param>
        /// <param name="location">The distinguished name of the OU in which the computer should be created.</param>
        /// <param name="description">The description of the new computer.</param>
        /// <returns>A ComputerPrincipal object representing the new computer.</returns>
        public static ComputerPrincipal CreateComputer(string name, string location, string description)
        {
            ComputerPrincipal newComputer = new ComputerPrincipal(GetPrincipalContext(location), name, GetRandomPassword(), true);
            newComputer.Description = description;
            newComputer.DisplayName = name;
            newComputer.Name = name;
            newComputer.Save();
            return newComputer;
        }

        /// <summary>
        /// Creates a new group in the specified location where the name, displayName, and 
        /// SamAccountName are the value of the name parameter.
        /// </summary>
        /// <param name="name">The new group's name, display name and SamAccountName.</param>
        /// <param name="location">The OU's distinguishedName where the new group should created.</param>
        /// <param name="description">The description of the new group.</param>
        /// <param name="scope">The GroupScope of the new group.</param>
        /// <param name="isSecurityGroup">A boolean designating the new group as a security group or not.</param>
        /// <returns>The GroupPrincipal object of the new group..</returns>
        public static GroupPrincipal CreateGroup(string name, string location, string description, GroupScope scope, bool isSecurityGroup)
        {
            GroupPrincipal newGroup = new GroupPrincipal(GetPrincipalContext(location), name);
            newGroup.GroupScope = scope;
            newGroup.IsSecurityGroup = isSecurityGroup;
            newGroup.Name = name;
            newGroup.DisplayName = name;
            newGroup.Description = description;
            newGroup.Save();
            return newGroup;
        }

        /// <summary>
        /// Creates a new user account in the specified OU. The new user has a random password and is disabled.
        /// </summary>
        /// <param name="firstName">The giveName of the new user.</param>
        /// <param name="lastName">The surname of the new user.</param>
        /// <param name="samAccountName">The SamAccountName of the new user. This must be unique.</param>
        /// <param name="location">The OU path/distinguished name in which the new user should be created.</param>
        /// <returns>The UserPrincipal object of the new user.</returns>
        public static UserPrincipal CreateUser(string firstName, string lastName, string samAccountName, string location)
        {
            UserPrincipal newUser = new UserPrincipal(GetPrincipalContext(location), samAccountName, GetRandomPassword(), false);
            newUser.GivenName = firstName;
            newUser.Surname = lastName;
            newUser.DisplayName = string.Format("{0}, {1}", lastName, firstName);
            newUser.UserPrincipalName = string.Format("{0}@{1}", samAccountName, GetCurrentDomain());
            newUser.Save();
            return newUser;
        }

        /// <summary>
        /// Deletes the specified computer account.
        /// </summary>
        /// <param name="name">The unique identifier of the computer.</param>
        /// <returns>True if the computer was deleted, false if not or the computer doesn't exist.</returns>
        public static Boolean DeleteComputer(string name)
        {
            try
            {
                ComputerPrincipal c = ComputerPrincipal.FindByIdentity(GetPrincipalContext(), name);
                c.Delete();
                c.Save();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes the specified group.
        /// </summary>
        /// <param name="name">The unique identifier of the group to delete.</param>
        /// <returns>True if the group was deleted, false otherwise.</returns>
        public static Boolean DeleteGroup(string name)
        {
            try
            {
                GroupPrincipal g = GroupPrincipal.FindByIdentity(GetPrincipalContext(), name);
                g.Delete();
                g.Save();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes the specified user account.
        /// </summary>
        /// <param name="name">The unique identifier of the user.</param>
        /// <returns>True if the user was deleted, false if not or the user doesn't exist.</returns>
        public static Boolean DeleteUser(string name)
        {
            try
            {
                UserPrincipal u = UserPrincipal.FindByIdentity(GetPrincipalContext(), name);
                u.Delete();
                u.Save();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the enabled boolean to false on the specified user account.
        /// </summary>
        /// <param name="name">The unique identifier of the user.</param>
        /// <returns>True if the account was disabled.</returns>
        public static Boolean DisableUser(string name)
        {
            UserPrincipal u = UserPrincipal.FindByIdentity(GetPrincipalContext(), name);
            try
            {
                u.Enabled = false;
                u.Save();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the enabled boolean to true on the specified user account.
        /// </summary>
        /// <param name="name">The unique identifier of the user.</param>
        /// <returns>True if the account was enabled, false otherwise.</returns>
        public static Boolean EnableUser(string name)
        {
            UserPrincipal u = UserPrincipal.FindByIdentity(GetPrincipalContext(), name);
            try
            {
                u.Enabled = true;
                u.Save();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Tests if the specified item exists. The item is either a user, computer, group,
        /// or OU.
        /// </summary>
        /// <param name="item">The unique identifier of the item.</param>
        /// <returns>True if the item exists, false otherwise.</returns>
        public static bool Exists(string item)
        {
            try
            {
                return (Principal.FindByIdentity(GetPrincipalContext(), item) != null) || (DirectoryEntry.Exists("LDAP://" + item));
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Expires the specified user or computer's password, forcing it to be required to be reset.
        /// </summary>
        /// <param name="name">The unique identifier of the user or computer.</param>
        /// <returns>True if the password was expired successfully, false otherwise.</returns>
        public static Boolean ExpirePassword(string name)
        {
            Boolean result = false;
            try
            {
                Principal p = Principal.FindByIdentity(GetPrincipalContext(), name);
            
                if ((p != null) && (p is UserPrincipal))
                {
                    UserPrincipal u = p as UserPrincipal;
                    u.ExpirePasswordNow();
                    u.Save();
                    result = true;
                }
                else if ((p != null) && (p is ComputerPrincipal))
                {
                    ComputerPrincipal c = p as ComputerPrincipal;
                    c.ExpirePasswordNow();
                    c.Save();
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// This method searches for and returns a ComputerPrincipal object that corrosponds to the specified computer name.
        /// </summary>
        /// <param name="name">The unique identifier for the computer.</param>
        /// <returns>A ComputerPrincipal object.</returns>
        public static ComputerPrincipal GetComputer(string name)
        {
            PrincipalContext pc = GetPrincipalContext();

            ComputerPrincipal cp = ComputerPrincipal.FindByIdentity(pc, name);
            return cp;
        }

        /// <summary>
        /// Returns a list of computers that are members of the specified group.
        /// </summary>
        /// <param name="group">The unique identifier of the group.</param>
        /// <returns>A list of ComputerPrincipal objects.</returns>
        public static List<ComputerPrincipal> GetComputersInGroup(string group)
        {
            List<ComputerPrincipal> results = new List<ComputerPrincipal>();
            GroupPrincipal g = GroupPrincipal.FindByIdentity(GetPrincipalContext(), group);
            PrincipalCollection members = g.Members;
            foreach (Principal member in members)
            {
                if (member is ComputerPrincipal)
                {
                    results.Add(GetComputer(member.SamAccountName));
                }
            }
            return results;
        }

        /// <summary>
        /// This method returns the domain of the current context, i.e., the domain of the user that is running the code.
        /// </summary>
        /// <returns>A string that is the current domain name.</returns>
        public static string GetCurrentDomain()
        {
            return Domain.GetCurrentDomain().Name;

        }

        /// <summary>
        /// Returns a list of distinct values of the specified property on users in the specified location.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static List<string> GetDistinctUserProperty(string property, string location)
        {
            List<string> results = new List<string>();
            DirectoryEntry searchRoot = new DirectoryEntry(string.Format("LDAP://{0}", location));
            DirectorySearcher search = new DirectorySearcher(searchRoot);
            search.Filter = "(&(objectCategory=person)(objectClass=user))";
            // lookup the correct property name. It's case sensitive.
            string targetProperty = PropertyLookup.Instance.get(property);
            search.PropertiesToLoad.Add(targetProperty);

            SearchResultCollection searchResults = search.FindAll();
            if (searchResults.Count > 0)
            {
                foreach (SearchResult r in searchResults)
                {
                    DirectoryEntry entry = r.GetDirectoryEntry();
                    if (entry.Properties.Contains(targetProperty))
                    {
                        string prop = entry.Properties[targetProperty].Value.ToString();
                        if (!results.Contains(prop))
                        {
                            results.Add(prop);
                        }
                    }

                }
            }
            results.Sort();
            return results;
        }

        /// <summary>
        /// Returns the distinguished name associated with the Principal identified by the specified name.
        /// </summary>
        /// <param name="name">The unique identifier of the Principal.</param>
        /// <returns>A string that is the distinguished name of the specified Principal.</returns>
        public static string GetDistinguishedName(string name)
        {
            Principal p = Principal.FindByIdentity(GetPrincipalContext(), name);
            if (p != null)
            {
                return p.DistinguishedName;
            }
            return string.Empty;
        }

        /// <summary>
        /// This method returns a list of Domain Controllers.
        /// </summary>
        /// <returns>A list of strings that are the Domain Controllers in the current Domain.</returns>
        public static List<string> GetDomainControllers()
        {
            List<string> results = new List<string>();
            Domain domain = Domain.GetCurrentDomain();
            foreach (DomainController dc in domain.DomainControllers)
            {
                results.Add(dc.Name);
            }
            return results;
        }

        /// <summary>
        /// This method returns a list of all Domains in the current Forest.
        /// </summary>
        /// <returns>A list of strings that are the names of the Domains in the current Forest.</returns>
        public static List<string> GetForestDomains()
        {
            List<string> alDomains = new List<string>();
            Forest currentForest = Forest.GetCurrentForest();
            DomainCollection myDomains = currentForest.Domains;

            foreach (Domain objDomain in myDomains)
            {
                alDomains.Add(objDomain.Name);
            }
            return alDomains;
        }

        /// <summary>
        /// This method returns the full domain name based on the friendly name. For example, if contoso is passed in
        /// contoso.com will be returned.
        /// </summary>
        /// <param name="friendlyDomainName">The friendly, or short domain name of the current domain.</param>
        /// <returns>A string that is the full domain name of the current domain.</returns>
        public static string GetFullDomainName(string friendlyDomainName)
        {
            string ldapPath = null;

            DirectoryContext objContext = new DirectoryContext(
                DirectoryContextType.Domain, friendlyDomainName);
            Domain objDomain = Domain.GetDomain(objContext);
            ldapPath = objDomain.Name;

            return ldapPath;
        }

        /// <summary>
        /// This method returns a list of Global Catalog servers in the current domain.
        /// </summary>
        /// <returns>A list of strings that are the names of the Global Catalog servers in the current domain.</returns>
        public static List<string> GetGlobalCatalogServers()
        {
            List<string> results = new List<string>();
            Forest currentForest = Forest.GetCurrentForest();
            foreach (GlobalCatalog gc in currentForest.GlobalCatalogs)
            {
                results.Add(gc.Name);
            }
            return results;
        }

        /// <summary>
        /// This method returns a GroupPrincipal object that corrosponds to the specified group name.
        /// </summary>
        /// <param name="name">The group to get</param>
        /// <returns>Returns a GroupPrincipal Object.</returns>
        public static GroupPrincipal GetGroup(string name)
        {
            PrincipalContext pc = GetPrincipalContext();
            return GroupPrincipal.FindByIdentity(pc, name);
        }

        /// <summary>
        /// Returns a list of GroupPrincipal objects that are in the specified OU.
        /// </summary>
        /// <param name="location">The distinguished name of the OU.</param>
        /// <returns>A list of GroupPrincipal objects that represent the groups in the specified OU.</returns>
        public static List<GroupPrincipal> GetGroups(string location)
        {
            List<GroupPrincipal> groups = new List<GroupPrincipal>();
            List<string> ouItems = GetItems(location);
            foreach (string item in ouItems)
            {
                GroupPrincipal g = GroupPrincipal.FindByIdentity(GetPrincipalContext(), item);
                if (g != null)
                {
                    groups.Add(g);
                }
            }
            return groups;
        }

        /// <summary>
        /// Returns a list of groups that are members of the specified group.
        /// </summary>
        /// <param name="group">The unique identifier of the group.</param>
        /// <returns>A list of GroupPrincipal objects.</returns>
        public static List<GroupPrincipal> GetGroupsInGroup(string group)
        {
            List<GroupPrincipal> results = new List<GroupPrincipal>();
            GroupPrincipal g = GroupPrincipal.FindByIdentity(GetPrincipalContext(), group);
            PrincipalCollection members = g.Members;
            foreach (Principal member in members)
            {
                if (member is GroupPrincipal)
                {
                    results.Add(GetGroup(member.SamAccountName));
                }
            }
            return results;
        }

        /// <summary>
        /// This method returns a list of GroupPrincipal objects that the specified AD object is a member of.
        /// </summary>
        /// <param name="name">The unique identifier of the Principal whose group membership will be returned.</param>
        /// <returns>A list of GroupPrincipal objects.</returns>
        public static List<GroupPrincipal> GetGroupMembership(string name)
        {
            List<GroupPrincipal> results = new List<GroupPrincipal>();
            Principal adObj = Principal.FindByIdentity(GetPrincipalContext(), name);
            foreach (GroupPrincipal p in adObj.GetGroups())
            {
                results.Add(p);
            }
            return results;
        }

        /// <summary>
        /// This method returns a list of items in the specified OU identified by the OU's distinguishedName.
        /// </summary>
        /// <param name="location">The distinguished name of the OU.</param>
        /// <returns>A list of strings that are the distinguishedNames of the objects in the specified OU.</returns>
        public static List<string> GetItems(string location)
        {
            List<string> results = new List<string>();

            DirectoryEntry directoryObject = new DirectoryEntry("LDAP://" + location);
            foreach (DirectoryEntry child in directoryObject.Children)
            {
                string childPath = child.Path.ToString();
                results.Add(childPath.Remove(0, 7));
                //remove the LDAP prefix from the path

                child.Close();
                child.Dispose();
            }
            directoryObject.Close();
            directoryObject.Dispose();

            return results;
        }

        /// <summary>
        /// Returns a PrincipalCollection that is the members of the specified groups.
        /// </summary>
        /// <param name="group">The unique identifier of the group.</param>
        /// <returns>A PrincipalCollection.</returns>
        public static PrincipalCollection GetMembers(string group)
        {
            GroupPrincipal g = GroupPrincipal.FindByIdentity(GetPrincipalContext(), group);
            return g.Members;
        }

        /// <summary>
        /// Gets the base principal context
        /// </summary>
        /// <returns>Retruns the PrincipalContext object</returns>
        private static PrincipalContext GetPrincipalContext()
        {
            return new PrincipalContext(ContextType.Domain, GetCurrentDomain());
        }

        /// <summary>
        /// This method returns a PrincipalContext targeted to the specified OU.
        /// </summary>
        /// <param name="location">The distinguished name of the container this context will be attached to.</param>
        /// <returns>A PrincipalContext object.</returns>
        private static PrincipalContext GetPrincipalContext(string location)
        {
            return new PrincipalContext(ContextType.Domain, GetCurrentDomain(), location);
        }

        /// <summary>
        /// This method will return the value of the specified property if it exists in the specified Principal's
        /// underlying DirectoryEntry object.
        /// </summary>
        /// <param name="principal">The unique identifier of a Principal.</param>
        /// <param name="property">The name of the Property whose value should be returned.</param>
        /// <returns></returns>
        public static String GetProperty(Principal principal, string property)
        {
            DirectoryEntry de = principal.GetUnderlyingObject() as DirectoryEntry;
            // property is case senstive, use PropertyLookup
            string targetProperty = PropertyLookup.Instance.get(property);
            if (de.Properties.Contains(targetProperty))
                return de.Properties[targetProperty].Value.ToString();
            else
                return String.Empty;
        }

        /// <summary>
        /// Overload for GetProperty(Principal, String)
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static String GetProperty(string principal, string property)
        {
            Principal p = Principal.FindByIdentity(GetPrincipalContext(), principal);
            return GetProperty(p, property);
        }

        /// <summary>
        /// Returns a 12 character password that has at least 2 special characters.
        /// </summary>
        /// <returns>
        /// A string that represents a random password 12 characters long that has at least
        /// 2 special characters.
        /// </returns>
        public static string GetRandomPassword()
        {
            return System.Web.Security.Membership.GeneratePassword(12, 2);
        }

        /// <summary>
        /// This method returns the full domain name as the top level ldap path. For example, if 
        /// the domain is blah.meh.com this method will return DC=blah,DC=meh,DC=com.
        /// </summary>
        /// <returns>A string representing the top level LDAP container of the current domain.</returns>
        public static string GetRootContainer()
        {
            string domain = GetCurrentDomain();
            string rootCn = String.Empty;
            string[] domainParts = domain.Split('.');
            foreach (string part in domainParts)
            {
                rootCn += string.Format("DC={0},", part);
            }
            return rootCn.TrimEnd(new char[] { ',' });
        }

        /// <summary>
        /// This method returns the SamAccountName associated with the principal identified by the specified name.
        /// </summary>
        /// <param name="name">The unique identifier of the Principal.</param>
        /// <returns>A string that is the SamAccountName of the Principal identified by the specified name.</returns>
        public static string GetSamAccountName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Principal p = Principal.FindByIdentity(GetPrincipalContext(), name);
                if (p != null)
                {
                    return p.SamAccountName;
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// This method returns a list of attributes that have values of the specified object identified by the name parameter.
        /// </summary>
        /// <param name="name">The unique identifier of the Principal whose attributes should be returned.</param>
        /// <returns>A dictionary of string keys and values where the keys are the names of the attributes and the
        /// values are, of course, the values of the attributes.</returns>
        public static SortedDictionary<string, string> GetUsedAttributes(string name)
        {
            Principal p = Principal.FindByIdentity(GetPrincipalContext(), name);
            String dn = p.DistinguishedName;
            DirectoryEntry objRootDSE = new DirectoryEntry("LDAP://" + dn);
            SortedDictionary<string, string> props = new SortedDictionary<string, string>();
            foreach (PropertyValueCollection prop in objRootDSE.Properties)
            {
                string value = String.Empty;
                if (prop.Value is Object[])
                {
                    foreach (var o in prop.Value as Object[])
                    {
                        value += o.ToString() + " ";
                    }
                }
                else if (prop.Value is String)
                {
                    value = prop.Value.ToString();
                }
                else
                {
                    value = prop.Value.ToString();
                }
                props.Add(prop.PropertyName, value);
            }
            return props;
        }

        /// <summary>
        /// Gets a certain user on Active Directory
        /// </summary>
        /// <param name="name">The unique identifier of the user.</param>
        /// <returns>Returns a UserPrincipal Object associated with the specified user.</returns>
        public static UserPrincipal GetUser(string name)
        {
            return UserPrincipal.FindByIdentity(GetPrincipalContext(), name);
        }

        /// <summary>
        /// Returns the properties and values sepcified properties of the specified user.
        /// </summary>
        /// <param name="name">The unique identifier of the user.</param>
        /// <param name="propertiesToLoad">
        /// A list of strings that are the names of the properties that
        /// should be returned.
        /// </param>
        /// <returns>
        /// A list of dictionaries where the key is the property name and the
        /// value is the property value.
        /// </returns>
        public static List<SortedDictionary<string, string>> GetUser(string name, List<string> propertiesToLoad)
        {
            List<SortedDictionary<string, string>> results = new List<SortedDictionary<string, string>>();
            SortedDictionary<string, string> allprops = GetUsedAttributes(name);

            foreach (string property in propertiesToLoad)
            {
                string targetProperty = PropertyLookup.Instance.get(property);
                if (
                    (!String.IsNullOrEmpty(targetProperty)) && 
                    (allprops.ContainsKey(targetProperty))
                    )
                {
                    SortedDictionary<string, string> prop = new SortedDictionary<string, string>();
                    prop.Add(targetProperty, allprops[targetProperty]);
                    results.Add(prop);
                }
            }

            return results;
        }

        /// <summary>
        /// This method searches for a user with the specified email, and if found, returns the UserPrincipal object
        /// associated with it.
        /// </summary>
        /// <param name="email">The email address that identifies the user.</param>
        /// <returns>A UserPrincipal object.</returns>
        public static UserPrincipal GetUserByEmail(string email)
        {
            UserPrincipal result = null;
            DirectorySearcher searcher = new DirectorySearcher();
            searcher.Filter = string.Format("(&(objectCategory=person)(objectClass=user)(mail={0}))", email);
            foreach (SearchResult r in searcher.FindAll())
            {
                DirectoryEntry de = r.GetDirectoryEntry();
                string sam = de.Properties["SamAccountName"].Value.ToString();
                string dn = de.Properties["distinguishedName"].Value.ToString();
                if (dn.Contains(",OU=Users,"))
                {
                    return GetUser(sam);
                }
            }
            return result;
        }

        /// <summary>
        /// This method returns a list of UserPrincipal objects that were found in the specified OU.
        /// </summary>
        /// <param name="location">The path of the OU.</param>
        /// <returns>A list of UserPrincipal objects.</returns>
        public static List<UserPrincipal> GetUsers(string location)
        {
            List<string> ouItems = GetItems(location);
            List<UserPrincipal> users = new List<UserPrincipal>();

            foreach (string item in ouItems)
            {
                UserPrincipal user = UserPrincipal.FindByIdentity(GetPrincipalContext(), item);
                if (user != null)
                {
                    users.Add(user);
                }
            }

            return users;
        }

        /// <summary>
        /// This method returns the members of the specified group that are UserPrincipals.
        /// </summary>
        /// <param name="group">The unique identifier of the group.</param>
        /// <returns>A list of UserPrincipal objects.</returns>
        public static List<UserPrincipal> GetUsersInGroup(string group)
        {
            List<UserPrincipal> results = new List<UserPrincipal>();
            GroupPrincipal g = GroupPrincipal.FindByIdentity(GetPrincipalContext(), group);
            PrincipalCollection members = g.Members;
            foreach (Principal member in members)
            {
                if (member is UserPrincipal)
                {
                    results.Add(GetUser(member.SamAccountName));
                }
            }
            return results;
        }

        /// <summary>
        /// Returns true if the specified principal is locked out.
        /// </summary>
        /// <param name="name">The unique identifier of the Principal.</param>
        /// <returns>
        /// True if the specified principal is a user or computer and is locked out,
        /// false otherwise.
        /// </returns>
        public static bool IsLockedOut(string name)
        {
            bool locked = false;
            Principal p = Principal.FindByIdentity(GetPrincipalContext(), name);
            if (p is UserPrincipal)
            {
                locked = (p as UserPrincipal).IsAccountLockedOut();
            }
            else if (p is ComputerPrincipal)
            {
                locked = (p as ComputerPrincipal).IsAccountLockedOut();
            }
            return locked;
        }

        /// <summary>
        /// This method returns true if the specified principal is a member of the specified group.
        /// </summary>
        /// <param name="principal">The unique identifier for the principal.</param>
        /// <param name="group">The unique identifier for the group.</param>
        /// <returns>True if the Principal is a member of the group, false otherwise.</returns>
        public static bool IsMember(string principal, string group)
        {
            GroupPrincipal g = GroupPrincipal.FindByIdentity(GetPrincipalContext(), group);
            Principal u = Principal.FindByIdentity(GetPrincipalContext(), principal);
            if (u != null && g != null)
            {
                // loop through the members of the group and compare samaccountnames to the user's.
                // have to do it this way rather than using UserPrincipal.IsMemberOf or GroupPrincipal.
                // Members.Contains because those have a known issue of returning false for Domain Users
                // and Domain Computers. Thanks, Microsoft.
                foreach (Principal p in g.Members)
                {
                    if (p.SamAccountName == u.SamAccountName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// This method will move an item to the specified OU identified by its distinguishedName.
        /// </summary>
        /// <param name="name">The unique identifier of the Principal to move.</param>
        /// <param name="location">The path, i.e., distinguished name of the OU to move the Principal to.</param>
        /// <returns>A Boolean. True if the object was moved, false otherwise.</returns>
        public static Boolean Move(string name, string location)
        {
            Boolean moved = false;

            if (Exists(name) && Exists(location))
            {
                DirectoryEntry source = Principal.FindByIdentity(GetPrincipalContext(), name).GetUnderlyingObject() as DirectoryEntry;
                DirectoryEntry target = new DirectoryEntry(("LDAP://" + location));

                source.MoveTo(target);
                moved = true;
            }

            return moved;
        }

        /// <summary>
        /// Removes the specified member from the specified group.
        /// </summary>
        /// <param name="member">The member to remove.</param>
        /// <param name="group">The group from which the member should be removed.</param>
        /// <returns>True if the member was removed, false otherwise.</returns>
        public static Boolean RemoveMember(string member, string group)
        {
            Boolean done = false;

            if (IsMember(member, group))
            {
                GroupPrincipal g = GroupPrincipal.FindByIdentity(GetPrincipalContext(), group);
                Principal p = Principal.FindByIdentity(GetPrincipalContext(), member);
                g.Members.Remove(p);
                g.Save();
                done = !IsMember(member, group);
            }
            return done;
        }

        /// <summary>
        /// Sets the password of the specified user to the specified password.
        /// </summary>
        /// <param name="name">The unique identifier of the user.</param>
        /// <param name="password">The new password for the user.</param>
        /// <returns>True if success, false otherwise.</returns>
        public static Boolean SetPassword(string name, string password)
        {
            try
            {
                UserPrincipal u = UserPrincipal.FindByIdentity(GetPrincipalContext(), name);
                u.SetPassword(password);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the specified property to the specified value on the Principal specified by name.
        /// </summary>
        /// <param name="name">The unique identifier of the Principal.</param>
        /// <param name="property">The name of the property.</param>
        /// <param name="value">The value to assign to the property.</param>
        /// <returns>True if success, false otherwise.</returns>
        public static Boolean SetProperty(string name, string property, string value)
        {
            Principal p = Principal.FindByIdentity(GetPrincipalContext(), name);
            return SetProperty(p, property, value);
        }

        /// <summary>
        /// Sets the specified property to the specified value on the specified Principal.
        /// </summary>
        /// <param name="principal">The Principal object.</param>
        /// <param name="property">The name of the property.</param>
        /// <param name="value">The value to assign to the property.</param>
        /// <returns>True if success, false otherwise.</returns>
        public static Boolean SetProperty(Principal principal, string property, string value)
        {
            DirectoryEntry de = principal.GetUnderlyingObject() as DirectoryEntry;
            // Use PropertyLookup to get the property name in the correct format.
            string targetProperty = PropertyLookup.Instance.get(property);

            // if the property is Manager the value should be a distinguishedName but those
            // are hard to pass around in URLs if they have errant commas and escaping backslashes.
            // Instead get the dn from the value which should be a samaccountname.
            if (targetProperty == PropertyLookup.Instance.get("manager"))
            {
                value = GetDistinguishedName(value);
            }
            if (de.Properties.Contains(targetProperty))
            {
                try
                {
                    de.Properties[targetProperty][0] = value;
                    de.CommitChanges();
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    de.Properties[targetProperty].Add(value);
                    de.CommitChanges();
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Unlocks the specified principal, which can be either a user or computer.
        /// </summary>
        /// <param name="principal">A unique identifier of a user or computer.</param>
        /// <returns>True if success, false otherwise.</returns>
        public static Boolean Unlock(string principal)
        {
            Principal p = Principal.FindByIdentity(GetPrincipalContext(), principal);
            try
            {
                if (p is UserPrincipal)
                {
                    UserPrincipal u = p as UserPrincipal;
                    u.UnlockAccount();
                    u.Save();
                }
                else if (p is ComputerPrincipal)
                {
                    ComputerPrincipal c = p as ComputerPrincipal;
                    c.UnlockAccount();
                    c.Save();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validates the username and password of a given user
        /// </summary>
        /// <param name="username">The username to validate</param>
        /// <param name="password">The password of the username to validate</param>
        /// <returns>Returns True of user is valid</returns>
        public static bool ValidateCredentials(string username, string password)
        {
            return GetPrincipalContext().ValidateCredentials(username, password);
        }

    }
}