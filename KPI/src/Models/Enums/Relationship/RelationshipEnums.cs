namespace KPISolution.Models.Enums.Relationship
{
    /// <summary>
    /// Represents the strength of a relationship between indicators or entities
    /// </summary>
    public enum RelationshipStrength
    {
        [Display(Name = "None")]
        None = 0,

        [Display(Name = "Very Weak")]
        VeryWeak = 1,

        [Display(Name = "Weak")]
        Weak = 2,

        [Display(Name = "Medium")]
        Medium = 3,

        [Display(Name = "Strong")]
        Strong = 4,

        [Display(Name = "Very Strong")]
        VeryStrong = 5,

        [Display(Name = "Direct")]
        Direct = 6
    }

    /// <summary>
    /// Represents the type of relationship between indicators or entities
    /// </summary>
    public enum RelationshipType
    {
        [Display(Name = "Contributes To")]
        ContributesTo = 0,

        [Display(Name = "Depends On")]
        DependsOn = 1,

        [Display(Name = "Correlates With")]
        CorrelatesWith = 2,

        [Display(Name = "Causes")]
        Causes = 3,

        [Display(Name = "Inhibits")]
        Inhibits = 4,

        [Display(Name = "Parent Of")]
        ParentOf = 5,

        [Display(Name = "Child Of")]
        ChildOf = 6,

        [Display(Name = "Mutual Influence")]
        MutualInfluence = 7,

        [Display(Name = "Overlaps With")]
        OverlapsWith = 8,

        [Display(Name = "Other")]
        Other = 9
    }
}