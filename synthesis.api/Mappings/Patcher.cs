using synthesis.api.Features.Project;

public static class Patcher
{
    public static T Patch<T>(T source, T destination)
    {
        if (source == null || destination == null)
        {
            throw new ArgumentNullException("invalid arguments for patcher");
        }

        foreach (var prop in source.GetType().GetProperties())
        {
            var value = prop.GetValue(source);

            if (value != null)
            {
                prop.SetValue(destination, value);
            }
        }

        return destination;
    }
}