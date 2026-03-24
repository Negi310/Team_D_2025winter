public class NameModel
{
    public SalmonData ApplyNameToSalmon(SalmonData targetSalmon, string decidedName)
    {
        targetSalmon.Name = decidedName;
        return targetSalmon;
    }
}