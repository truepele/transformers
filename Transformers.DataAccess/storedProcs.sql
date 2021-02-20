CREATE PROCEDURE [calc_overall_score]
    @Courage tinyint,
    @Endurance tinyint,
    @Firepower tinyint,
    @Intelligence tinyint,
    @Rank tinyint,
    @Skill tinyint,
    @Speed tinyint,
    @Strength tinyint
AS
SELECT @Strength + @Speed + @Skill + @Rank + @Firepower + @Intelligence + @Endurance + @Courage as 'OverallRank';
