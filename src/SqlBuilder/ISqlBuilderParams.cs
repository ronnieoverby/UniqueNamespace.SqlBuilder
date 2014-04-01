namespace UniqueNamespace
{
    public interface ISqlBuilderParams<in TParamsIn, out TParamsOut>
        where TParamsIn : class
        where TParamsOut : class
    {
        void Expand(TParamsIn parameters);
        TParamsOut Materialize();
    }
}