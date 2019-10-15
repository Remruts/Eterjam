using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : MonoBehaviour
{
  PlayerScript player;
  geneticLottery strategyBag;

  float currentMoveDirection = 0f;

  CustomTimer reactionTimer;
  CustomTimer planningTimer;
  CustomTimer mutationTimer;

  strategy currentStrategy;
  int currentAction = 0;
  Vector2 shootVector = Vector2.zero;
  
  // Start is called before the first frame update
  void Start(){
    player = GetComponent<PlayerScript>();
    strategyBag = MatchManager.match.getStrategies(player.team);
    
    reactionTimer = new CustomTimer(0.2f, react);
    mutationTimer = new CustomTimer(0.5f, strategyBag.darwin);
    planningTimer = new CustomTimer(1f, plan);

    plan();
  }

  // Update is called once per frame
  void Update(){
    if (!player.cpu){
      return;
    }
    reactionTimer.tick(reactionTimer.getDuration);
    planningTimer.tick(planningTimer.getDuration);
    
    player.move(currentMoveDirection);
    player.shootLogic(shootVector);
  }

  void react(){
    if (currentStrategy == null){
      return;
    }
    strategyAction newAction = currentStrategy.getAction(currentAction);

    shootVector = newAction.shoot_vector;
    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
    GameObject currentEnemy = null;
    foreach (var p in players){
      if (p.GetComponent<PlayerScript>().team != player.team){
        currentEnemy = p;
        break;
      }
    }

    Vector2 dashVector = newAction.dash_vector;
    currentMoveDirection = newAction.move_direction;

    if (currentEnemy != null){
      float targetPlayer = newAction.targetPlayer;
      Vector2 enemyVector = (transform.position - currentEnemy.transform.position).normalized;

      if (targetPlayer < 0f){
        enemyVector.y = -enemyVector.y;
        targetPlayer = Mathf.Abs(targetPlayer);
      }

      float newMagnitude = Mathf.Lerp(shootVector.magnitude, enemyVector.magnitude, targetPlayer);

      float newAngle = Angle360.lerp(Mathf.Atan2(shootVector.y, shootVector.x) * Mathf.Rad2Deg,Mathf.Atan2(enemyVector.y, enemyVector.x) * Mathf.Rad2Deg, targetPlayer / 2.0f) * Mathf.Deg2Rad;
      
      shootVector = new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle)) * newMagnitude;

      newAngle = Angle360.lerp(Mathf.Atan2(dashVector.y, dashVector.x) * Mathf.Rad2Deg, Mathf.Atan2(-enemyVector.y, -enemyVector.x * Mathf.Sign(newAction.targetPlayer)) * Mathf.Rad2Deg, targetPlayer / 2.0f) * Mathf.Deg2Rad;

      dashVector = new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle)).normalized;
      currentMoveDirection = Mathf.Sign(Mathf.Lerp(currentMoveDirection, -enemyVector.x * Mathf.Sign(newAction.targetPlayer), targetPlayer));
    }
    
    if (newAction.dash){
      player.doADash(dashVector);
    }

    if (newAction.jump){
      player.jump();
    }

    currentStrategy.updateScore(currentAction, -Time.deltaTime);

    currentAction = (currentAction + 1) % currentStrategy.Size();
  }

  public void addScoreToAI(float score){
    currentStrategy.updateScore(currentAction, score);
  }

  void plan(){
    if (currentStrategy != null){
      strategyBag.addStrategy(currentStrategy);
    }
    currentStrategy = strategyBag.draw();
    currentAction = 0;
  }
}

public class geneticLottery{

  List<strategy> theBag;

  int bagSize = 10;
  int pickNumber = 3;

  int iterations = 0;
  int maxIterations = 5;
  public geneticLottery(){
    pickNumber = Mathf.Clamp(pickNumber, 1, bagSize);
    theBag = new List<strategy>();
    for (int i=0; i < bagSize; ++i){
      theBag.Add(new strategy(bagSize));
    }
  }
  
  public strategy draw(){
    if (theBag.Count < bagSize){
      theBag.Add(new strategy(bagSize));
    }
    int index = Random.Range(0, pickNumber);

    theBag.Sort((strat1, strat2) => strat2.getScore().CompareTo(strat1.getScore()));

    /*
    iterations++;
    if (iterations > maxIterations){
      darwin();
      iterations = 0;
    }
     */

    strategy theChosenOne = theBag[index];
    theBag.RemoveAt(index);
    
    return theChosenOne;
  }

  public void darwin(){
    theBag.Sort((strat1, strat2) => strat2.getScore().CompareTo(strat1.getScore()));
    //theBag.RemoveRange(pickNumber, bagSize);
    List<strategy> newBag = new List<strategy>();

    for (int i=0; i < pickNumber && newBag.Count < bagSize; i++){
      for (int j=0; j < pickNumber && newBag.Count < bagSize; j++){
        if (i!=j){
          newBag.Add(new strategy(theBag[i], theBag[j]));
        }
      }
    }
    while (newBag.Count < bagSize){
      newBag.Add(new strategy(bagSize));
    }
    
    theBag = newBag;
  }

  public void addStrategy(strategy theStrat){
    theBag.Add(theStrat);
  }
}

public class strategy{
  List<strategyAction> actionList;
  int size = 0;
  public strategy(int newSize){
    actionList = new List<strategyAction>();
    bool chance = Random.Range(0f, 1f) > 0.8f;
    for (int i=0; i < newSize; i++){
      strategyAction newAction = new strategyAction();
      if (chance){
        newAction.targetPlayer = -newAction.targetPlayer;
      }
      actionList.Add(newAction);
    }
    
    actionList.Sort((act1, act2) => act2.score.CompareTo(act1.score));
    size = newSize;
  }

  public strategy(strategy st1, strategy st2){
    actionList = new List<strategyAction>();
    int newSize = Mathf.Min(st1.actionList.Count, st2.actionList.Count); ;
    for(int i = 0; i < newSize; i++){
      actionList.Add(new strategyAction(st1.actionList[i], st2.actionList[i]));
    }
    actionList.Sort((act1, act2) => act2.score.CompareTo(act1.score));
    size = newSize;
  }

  public int Size(){
    return size;
  }

  public float getScore(){
    float sum = 0f;
    foreach (var strat in actionList){
      sum += strat.score;
    }
    return sum;
  }

  public void updateScore(int i, float score){
    actionList[i].score += score;
  }

  public strategyAction getAction(int i){
    return actionList[i];
  }
}

public class strategyAction{
  [Range(-1f, 1f)]
  public float move_direction;
  public bool jump;
  public bool dash;
  public float targetPlayer;
  public Vector2 shoot_vector;
  public Vector2 dash_vector;

  public float score;

  public strategyAction(){
    move_direction = Random.Range(-1f, 1f);
    
    jump = (Random.Range(0f, 1f) > 0.7f);
    
    dash  = (Random.Range(0f, 1f) > 0.9f);

    if (jump){
      dash = false;
    }
    if (dash){
      jump = false;
    }
    
    if (Random.Range(0f, 1f) > 0.5f){
      shoot_vector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
      targetPlayer  = Random.Range(0.9f, 1f);
    } else {
      shoot_vector = Vector2.zero;
      targetPlayer  = 0f;
    }
    dash_vector  = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

    score = Random.Range(-0.25f, 0.25f);
  }

  //Crossover
  public strategyAction(strategyAction st1, strategyAction st2){
    float dominance = 0.8f;
    if (st1.score > st2.score){
      dominance = 1f - dominance;
    }
    
    float chance = Random.Range(0f, 1f);      
    move_direction = (chance > dominance) ? st1.move_direction : st2.move_direction;
    
    chance = Random.Range(0f, 1f);
    jump = (chance > dominance) ? st1.jump : st2.jump;

    chance = Random.Range(0f, 1f);
    dash = (chance > dominance) ? st1.dash : st2.dash;

    if (jump){
      dash = false;
    }
    if (dash){
      jump = false;
    }

    chance = Random.Range(0f, 1f);
    targetPlayer = (chance > dominance) ? Mathf.Lerp(st1.targetPlayer, st2.targetPlayer, 0.8f) : Mathf.Lerp(st1.targetPlayer, st2.targetPlayer, 0.2f);

    chance = Random.Range(0f, 1f);
    shoot_vector = (chance > dominance) ? Vector2.Lerp(st1.shoot_vector, st2.shoot_vector, 0.8f) : Vector2.Lerp(st1.shoot_vector, st2.shoot_vector, 0.2f);

    chance = Random.Range(0f, 1f);
    dash_vector = (chance > dominance) ? Vector2.Lerp(st1.dash_vector, st2.dash_vector, 0.8f) : Vector2.Lerp(st1.dash_vector, st2.dash_vector, 0.2f);

    score = (st1.score + st2.score) / 2.0f;

    chance = Random.Range(0f, 1f);
    if (chance > 0.5f){
      mutate();
    }
  }

  public void mutate(){
    move_direction = Mathf.Clamp(move_direction + Random.Range(-0.1f, 0.1f), -1f, 1f);
    if (Random.Range(0f, 1f) > 0.9f){
      jump = !jump;
    }
    if (Random.Range(0f, 1f) > 0.9f){
      dash = !dash;
    }
    if (Random.Range(0f, 1f) > 0.9f){
      targetPlayer = Mathf.Clamp(targetPlayer + Random.Range(-0.1f, 0.1f), 0f, 1f);
    }
    if (Random.Range(0f, 1f) > 0.9f){
      targetPlayer = -targetPlayer;
    }

    if (jump){
      dash = false;
    }
    if (dash){
      jump = false;
    }

    shoot_vector = new Vector2(
      Mathf.Clamp(shoot_vector.x + Random.Range(-0.05f, 0.05f), -1f, 1f),
      Mathf.Clamp(shoot_vector.x + Random.Range(-0.05f, 0.05f), -1f, 1f));
    
    dash_vector = new Vector2(
      Mathf.Clamp(dash_vector.x + Random.Range(-0.2f, 0.2f), -1f, 1f),
      Mathf.Clamp(dash_vector.x + Random.Range(-0.2f, 0.2f), -1f, 1f));
  }
}