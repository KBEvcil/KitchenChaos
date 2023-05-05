using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangeEventArgs> OnStateChange;
    public class OnStateChangeEventArgs : EventArgs
    {
        public State state;
    }
    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned
    }
    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

    private State currentState;

    private float fryingTimer;
    private float burningTimer;
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;

    private void Start()
    {
        currentState = State.Idle;
    }

    private void Update()
    {
        
        if(HasKitchenObject())
        {
            switch (currentState)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
                    });

                    if (fryingTimer > fryingRecipeSO.fryingTimerMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);
                        burningTimer = 0f;

                        currentState = State.Fried;
                        OnStateChange?.Invoke(this, new OnStateChangeEventArgs
                        {
                            state = currentState
                        });

                        burningRecipeSO = GetBurningRecipeWithInput(GetKitchenObject().GetKitchenObjectSO());
                    }
                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = burningTimer / burningRecipeSO.burningTimerMax
                    });

                    if (burningTimer > burningRecipeSO.burningTimerMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                        currentState = State.Burned;
                        OnStateChange?.Invoke(this, new OnStateChangeEventArgs
                        {
                            state = currentState
                        });
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 1
                        });
                    }
                    break;
                case State.Burned:
                    break;

            }
            
        }
    }
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {

            if (player.HasKitchenObject())
            {
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    fryingRecipeSO = GetFryingRecipeWithInput(GetKitchenObject().GetKitchenObjectSO());

                    currentState = State.Frying;
                    OnStateChange?.Invoke(this, new OnStateChangeEventArgs
                    {
                        state = currentState
                    });

                    fryingTimer = 0f;
                }
            }
        }
        else
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if(plateKitchenObject.TryAddIngredients(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();

                        currentState = State.Idle;
                        OnStateChange?.Invoke(this, new OnStateChangeEventArgs
                        {
                            state = currentState
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0
                        });
                    }
                    
                }
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent(player);

                currentState = State.Idle;
                OnStateChange?.Invoke(this, new OnStateChangeEventArgs
                {
                    state = currentState
                });

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0
                });

            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO input)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeWithInput(input);
        if (fryingRecipeSO)
            return true;
        return false;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO input)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeWithInput(input);
        if (fryingRecipeSO)
            return fryingRecipeSO.output;
        return null;
    }

    private FryingRecipeSO GetFryingRecipeWithInput(KitchenObjectSO input)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == input)
            {
                return fryingRecipeSO;
            }
        }
        return null;
    }
    private BurningRecipeSO GetBurningRecipeWithInput(KitchenObjectSO input)
    {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
        {
            if (burningRecipeSO.input == input)
            {
                return burningRecipeSO;
            }
        }
        return null;
    }

    public bool IsFried()
    {
        return currentState == State.Fried;
    }
}
