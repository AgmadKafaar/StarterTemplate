using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Novell.Directory.Ldap;
using StarterTemplate.Core.Entities;
using System;

namespace StarterTemplate.Core.Services.Authentication
{
    public interface ILdapService
    {
        string Domain { set; get; }

        string SearchBaseDn { set; get; }

        User Login(string username, string password);
    }

    public class LdapService : ILdapService
    {
        private readonly string[] _attributesToReturn;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LdapService> _logger;
        public LdapService(IConfiguration configuration, ILogger<LdapService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _attributesToReturn = new string[] { "cn", "name", "mail" };
        }

        public string Domain { get; set; }
        public string SearchBaseDn { get; set; }
        public User Login(string username, string password)
        {
            try
            {
                // if they are not set, return nothing and exit early.
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return null;

                // if the ad domain is prepended, just strip it from the username.
                if (username.StartsWith($"{Domain}\\", StringComparison.OrdinalIgnoreCase))
                    username = username.Replace($"{Domain}\\", string.Empty, StringComparison.OrdinalIgnoreCase);

                //1. Connect to LDAP
                //2. Perform Ldap Bind
                using var connection = new LdapConnection();
                LdapSearchConstraints cons = connection.SearchConstraints;
                cons.ReferralFollowing = true;
                connection.Constraints = cons;
                //389 is the default port
                connection.Connect(_configuration["LdapURL"], 389);
                connection.Bind(LdapConnection.LdapV3, $"{Domain}\\{username}", password);
                if (!connection.Bound) return null;

                string searchFilter = $"(samaccountname={username})";

                //3. Do an Ldap search for email and agent name
                // using the SamAccountName name as the QueryByFilter.
                var searchResults = connection.Search(SearchBaseDn, LdapConnection.ScopeSub, searchFilter, _attributesToReturn, false);
                // Grab the user from the searcher
                if (searchResults.HasMore())
                {
                    LdapEntry nextEntry = null;
                    try
                    {
                        nextEntry = searchResults.Next();
                    }
                    catch (LdapException psde)
                    {
                        _logger.LogError(psde, $"Cannot find user: {username}");
                        return null;
                    }

                    LdapAttributeSet attributeSet = nextEntry.GetAttributeSet();
                    var name = attributeSet.GetAttribute("name")?.StringValue;
                    var email = attributeSet.GetAttribute("mail")?.StringValue;
                    //if found, the proceed to map the userPrincipal object to agent record on Freshdesk database.
                    return new User(name, email);
                }
                else
                {
                    return null;
                }
            }
            catch (LdapException psde)
            {
                _logger.LogError(psde, "Unable to connect to AD server");
                return null;
            }
        }
    }
}