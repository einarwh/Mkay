namespace Mkay
{
    public interface ITreeBuilder<TNode>
    {
        TNode Build<T>(Atom<T> atom);

        TNode Build(OpData<TNode> data);
    }
}