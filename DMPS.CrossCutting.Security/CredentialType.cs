namespace DMPS.CrossCutting.Security.Enums
{
    /// <summary>
    /// Defines the types of credentials that can be stored and retrieved
    /// by the Windows Credential Manager. This mirrors the native CRED_TYPE constants.
    /// </summary>
    public enum CredentialType
    {
        /// <summary>
        /// A generic credential, typically used for application-specific username/password pairs.
        /// Corresponds to CRED_TYPE_GENERIC.
        /// </summary>
        Generic = 1,

        /// <summary>
        /// A domain password credential (user, domain, password).
        /// Corresponds to CRED_TYPE_DOMAIN_PASSWORD.
        /// </summary>
        DomainPassword = 2,

        /// <summary>
        /// A domain certificate credential.
        /// Corresponds to CRED_TYPE_DOMAIN_CERTIFICATE.
        /// </summary>
        DomainCertificate = 3,

        /// <summary>
        /// A domain-visible generic credential.
        /// Corresponds to CRED_TYPE_DOMAIN_VISIBLE_PASSWORD.
        /// </summary>
        DomainVisiblePassword = 4,

        /// <summary>
        /// A generic certificate credential.
        /// Corresponds to CRED_TYPE_GENERIC_CERTIFICATE.
        /// </summary>
        GenericCertificate = 5
    }
}