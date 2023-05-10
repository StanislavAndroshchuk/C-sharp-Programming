namespace Task2_Class;

public abstract class State
{
    protected IContext _context;

    public State(IContext context)
    {
        _context = context;
    }
    public abstract void Publishing(User user);

    public static State ConvertEnum(StatesEnum enumName, IContext context)
    {
        State state = null!;
        switch (enumName)
        {
            case StatesEnum.Draft:
                state = new DraftState(context);
                break;
            case StatesEnum.Moderation:
                state = new ModerationState(context);
                break;
            case StatesEnum.Published:
                state = new PublishedState(context);
                break;
        }

        return state;
    }
}
public class DraftState : State
{
    public DraftState(IContext context) : base(context) { }
    

    public override void Publishing(User user)
    {
        if (user.IsAdmin())
        {
            _context.ChangeState(StatesEnum.Published);
        }

        if (user.IsManager())
        {
            _context.ChangeState(StatesEnum.Moderation);
        }
    }
}

public class ModerationState : State
{
    public ModerationState(IContext context) : base(context) { }

    public override void Publishing(User user)
    {
        if (user.IsAdmin())
        {
            _context.ChangeState(StatesEnum.Published);
        }
    }
}

public class PublishedState : State
{
    public PublishedState(IContext context) : base(context) { }

    public override void Publishing(User user) {}
}
