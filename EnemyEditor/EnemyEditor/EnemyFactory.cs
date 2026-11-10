using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public class EnemyFactory
    {
        public static IEnemy CreateEnemy(string typeName, params object[] args)
        {
            var type = Assembly.GetExecutingAssembly()
                .GetTypes()
                .FirstOrDefault(t => t.Name == typeName);

            if (type == null)
            {
                throw new ArgumentException($"Тип {typeName} не найден!");
            }

            return (IEnemy)Activator.CreateInstance(type, args)!;
        }
    }
}
