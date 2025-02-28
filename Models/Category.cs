namespace DemoDishOfProj.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Dish> Dishes = new List<Dish>();
    }
}
