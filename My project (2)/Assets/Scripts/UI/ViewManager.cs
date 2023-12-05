using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour
{
    public static ViewManager Instance;

    [SerializeField] private View startingView;

    [SerializeField] private View[] views;

    private View currentView;

    private readonly Stack<View> history = new Stack<View>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < views.Length; i++)
        {
            views[i].Initialize();
            views[i].Hide();
        }

        if (startingView != null)
        {
            Show(startingView, false);
        }
    }

    public static T GetView<T>() where T : View
    {
        for (int i = 0; i < Instance.views.Length; i++)
        {
            if (Instance.views[i] is T tView)
            {
                return tView;
            }
        }

        return null;
    }

    public static void Show<T>(bool remenber = true) where T : View
    {
        for (int i = 0;i < Instance.views.Length; i++)
        {
            if (Instance.views[i] is T)
            {
                if (Instance.currentView != null)
                {
                    if (remenber)
                    {
                        Instance.history.Push(Instance.currentView);
                    }

                    Instance.currentView.Hide();
                }

                Instance.views[i].Show();

                Instance.currentView = Instance.views[i];
            }
        }
    }

    public static void Show(View view, bool remenber = true)
    {
        if (Instance.currentView != null)
        {
            if (remenber)
            {
                Instance.history.Push(Instance.currentView);
            }

            Instance.currentView.Hide();
        }

        view.Show();

        Instance.currentView = view;
    }

    public static void ShowLast() 
    {
        if (Instance.history.Count != 0)
        {
            Show(Instance.history.Pop(), false);
        }
    }
}
