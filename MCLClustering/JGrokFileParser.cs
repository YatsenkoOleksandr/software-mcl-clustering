using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ClusteringEntities.Core;

namespace MCLClustering
{
    public class JGrokFileParser : IEntityParser
    {
        private readonly string filename;

        // "entity name"
        private const string NAME_REGEX_STRING = "\"[\\d\\w\\.\\$]+\"";

        // Regex: "dependent entity name"->"dependence entity name"
        private const string CLASSMETHOD_REGEX_STRING = NAME_REGEX_STRING + "->" + NAME_REGEX_STRING;

        public JGrokFileParser(string filename)
        {
            this.filename = filename;
        }

        public ICollection<Entity> Parse()
        {
            // keeps class dependecies as a map
            Dictionary<string, List<string>> tempClassDependecies = new Dictionary<string, List<string>>();

            Regex classMethodRelationRegex = new Regex(CLASSMETHOD_REGEX_STRING);
            Regex nameRegex = new Regex(NAME_REGEX_STRING);

            using (StreamReader textReader = new StreamReader(filename))
            {
                string line;
                string leftClass = string.Empty;
                string rightClass = string.Empty;
                while ((line = textReader.ReadLine()) != null)
                {
                    if (classMethodRelationRegex.IsMatch(line))
                    {
                        MatchCollection matches = nameRegex.Matches(line);
                        if (matches.Count == 2)
                        {
                            leftClass = matches[0].Value;
                            rightClass = matches[1].Value;

                            if (!tempClassDependecies.ContainsKey(leftClass))
                            {
                                tempClassDependecies.Add(leftClass, new List<string>());
                            }
                            tempClassDependecies[leftClass].Add(rightClass);

                            if (!tempClassDependecies.ContainsKey(rightClass))
                            {
                                tempClassDependecies.Add(rightClass, new List<string>());
                            }
                        }
                    }
                }
            }

            ICollection<Entity> parsedEntities = new List<Entity>();

            foreach(var dependentClassName in tempClassDependecies.Keys)
            {
                List<string> dependencyClassNames = tempClassDependecies[dependentClassName];

                ICollection<EntityRelation> entityRelations = new List<EntityRelation>();
                foreach(var dependencyClassName in dependencyClassNames)
                {
                    entityRelations.Add(
                        new EntityRelation()
                        {
                            DependentEntity = dependentClassName,
                            DependenceEntity = dependencyClassName,
                            Weight = 1
                        });
                }

                parsedEntities.Add(
                    new Entity()
                    {
                        Name = dependentClassName,
                        Relations = entityRelations
                    });
            }

            return parsedEntities;
        }
    }
}
