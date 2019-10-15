using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Xml.Serialization;

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
    mutationTimer = new CustomTimer(3f, mutate);
    planningTimer = new CustomTimer(2f, plan);

    plan();
  }

  // Update is called once per frame
  void Update(){
    if (!player.cpu || MatchManager.match.paused){
      return;
    }
    reactionTimer.tick(reactionTimer.getDuration);
    planningTimer.tick(planningTimer.getDuration);
    mutationTimer.tick(mutationTimer.getDuration);
    
    player.move(currentMoveDirection);
    player.shootLogic(shootVector);
  }

  public void mutate(){
    //Debug.Log("TEENAGE MUTANT NINJA TURTLES!");
    strategyBag.darwin();
    mutationTimer.resetTimerInstantly();
    //mutationTimer.setDuration(1f);
  }

  public void giveMoreTime(){
    mutationTimer.resetTimerInstantly();
    planningTimer.resetTimerInstantly();
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

    GameObject currentProjectile = findClosestProjectile();

    Vector2 dashVector = newAction.dash_vector;
    currentMoveDirection = newAction.move_direction;

    bool willDash = newAction.dash;

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

    if (newAction.targetPlayer < 0.5f && currentProjectile != null){
      float targetProjectile = newAction.targetProjectile;
      Vector2 projectileVector = (transform.position - currentProjectile.transform.position).normalized;

      if (targetProjectile < 0f){
        projectileVector.y = -projectileVector.y;
        targetProjectile = Mathf.Abs(targetProjectile);
      }

      float newMagnitude = Mathf.Lerp(shootVector.magnitude, projectileVector.magnitude, targetProjectile);

      willDash = newAction.dashProbability > 0.8f;

      float newAngle;
      if (!willDash){
        newAngle = Angle360.lerp(Mathf.Atan2(shootVector.y, shootVector.x) * Mathf.Rad2Deg,Mathf.Atan2(projectileVector.y, projectileVector.x) * Mathf.Rad2Deg, targetProjectile / 2.0f) * Mathf.Deg2Rad;
      
        shootVector = new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle)) *   newMagnitude;
      }

      newAngle = Angle360.lerp(Mathf.Atan2(dashVector.y, dashVector.x) * Mathf.Rad2Deg, Mathf.Atan2(projectileVector.y, projectileVector.x * Mathf.Sign(newAction.targetProjectile)) * Mathf.Rad2Deg, targetProjectile / 2.0f) * Mathf.Deg2Rad;

      dashVector = new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle)).normalized;

      currentMoveDirection = Mathf.Sign(Mathf.Lerp(currentMoveDirection, projectileVector.x * Mathf.Sign(newAction.targetProjectile), targetProjectile));
    }
    
    if (willDash){
      player.doADash(dashVector);
    }

    bool willJump = newAction.jump;
    if (willDash){
      willJump = false;
    }

    if (willJump){
      player.jump();
    }

    addScoreToAI(-Time.deltaTime * 2f);

    currentAction = (currentAction + 1) % currentStrategy.Size();
  }

  GameObject findClosestProjectile(){
    GameObject currentProjectile = null;
    GameObject[] projectiles = GameObject.FindGameObjectsWithTag("projectile");
    float minDistance = 9f;
    foreach (var p in projectiles){
      if (p.GetComponent<ProjectileScript>().team == player.team){
        continue;
      }
      float currentDistance = (p.transform.position - transform.position).sqrMagnitude;
      if (currentDistance < minDistance){
        minDistance = currentDistance;
        currentProjectile = p;
      }
    }
    
    return currentProjectile;
  }

  public void addScoreToAI(float score){
    currentStrategy.updateScore(currentAction, score);
  }

  public void plan(){
    if (currentStrategy != null){
      strategyBag.addStrategy(currentStrategy);
    }
    currentStrategy = strategyBag.draw();
    currentAction = 0;
  }
}

public class geneticLottery{

  List<strategy> theBag;

  int bagSize = 20;
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
    int index = UnityEngine.Random.Range(0, pickNumber);

    if (UnityEngine.Random.Range(0f, 1f) > 0.95f){
        index = UnityEngine.Random.Range(0, theBag.Count);
    } 

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

  public void saveStrategies(string jsonSavePath){
    //string jsonSavePath = Application.persistentDataPath + "/ai.json";
    string resultString = JsonHelper.ToJson(theBag.ToArray(), true);
    //Debug.Log(resultString);
    File.WriteAllText (jsonSavePath, resultString);
  }

  public void loadStrategies(string jsonSavePath){
    List<strategy> newBag = new List<strategy>();
    try {
      string jsonString = File.ReadAllText(jsonSavePath);
      newBag = new List<strategy>(JsonHelper.FromJson<strategy>(jsonString));
    } catch{
      Debug.Log("No pudimos cargar la AI");
    } finally {
      if (newBag.Count > 0){
        theBag = newBag;
        Debug.Log("Éxito al cargar la AI");
      }
      //Debug.Log(theBag[0].getAction(0).ToString());
    }
  }

  public void darwin(){
    theBag.Sort((strat1, strat2) => strat2.getScore().CompareTo(strat1.getScore()));
    foreach (var meh in theBag){
      if (meh.getScore() > 100f){
        Debug.Log(meh.getScore());
      }
    }
    //theBag.RemoveRange(pickNumber, bagSize);
    List<strategy> newBag = new List<strategy>();

    for (int i=0; i < pickNumber && newBag.Count < bagSize; i++){
      for (int j=i; j < pickNumber && newBag.Count < bagSize; j++){
        if (i!=j){
          newBag.Add(new strategy(theBag[i], theBag[j]));
        }
      }
    }
    for (int i = newBag.Count; i < theBag.Count; i++){
      newBag.Add(theBag[i]);
    }
    while (newBag.Count < bagSize){
      newBag.Add(new strategy(bagSize));
    }
    // Agregar algo de randomness
    newBag[newBag.Count - 1] = new strategy(bagSize);

    theBag = newBag;
  }

  public void addStrategy(strategy theStrat){
    theBag.Add(theStrat);
  }
}

[Serializable]
public class strategy{
  [SerializeField]
  List<strategyAction> actionList;
  [SerializeField]
  int size = 0;
  float maxScore = 100f;
  float minScore = -30f;
  public strategy(int newSize){
    actionList = new List<strategyAction>();
    bool chance = UnityEngine.Random.Range(0f, 1f) > 0.8f;
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
    int newSize = Mathf.Min(st1.actionList.Count, st2.actionList.Count);
    for(int i = 0; i < newSize; i++){
      actionList.Add(new strategyAction(st1.actionList[i], st2.actionList[i]));
    }
    if (UnityEngine.Random.Range(0f, 1f) > 0.99f){
        actionList[actionList.Count-1] = new strategyAction();
        actionList[actionList.Count-1].score += 5f;
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
    int cutoff = actionList.Count / 3;
    for (int j = 0; j < cutoff; j++){
      int index = (i - j < 0) ? actionList.Count + i - j : i - j;
      float theScore = (actionList[index % actionList.Count].score + score * (1f-j/(float)cutoff));
      actionList[index % actionList.Count].score = Mathf.Clamp(theScore, minScore, maxScore);
    }
  }

  public strategyAction getAction(int i){
    return actionList[i];
  }
}

[Serializable]
public class strategyAction{
  [Range(-1f, 1f)]
  public float move_direction;
  public bool jump;
  public bool dash;
  public float targetPlayer;
  public float targetProjectile;
  public float dashProbability;
  public Vector2 shoot_vector;
  public Vector2 dash_vector;
  
  public float score;

  float maxScore = 100f;
  float minScore = -30f;

  override public string ToString(){
    string str;
    str = JsonUtility.ToJson(this);
    return str;
  }

  public strategyAction(){
    move_direction = UnityEngine.Random.Range(-1f, 1f);
    
    jump = (UnityEngine.Random.Range(0f, 1f) > 0.7f);
    
    dash  = (UnityEngine.Random.Range(0f, 1f) > 0.9f);

    dashProbability = UnityEngine.Random.Range(0.5f, 0.6f);

    if (jump){
      dash = false;
    }
    if (dash){
      jump = false;
    }
    
    if (UnityEngine.Random.Range(0f, 1f) > 0.5f){
      shoot_vector = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
      targetPlayer  = UnityEngine.Random.Range(0.9f, 1f);
      targetProjectile  = UnityEngine.Random.Range(0.9f, 1f);
    } else {
      shoot_vector = Vector2.zero;
      targetPlayer  = 0f;
    }
    
    dash_vector  = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));

    score = UnityEngine.Random.Range(-0.25f, 0.25f);
  }

  //Crossover
  public strategyAction(strategyAction st1, strategyAction st2){
    float dominance = 0.8f;
    if (st1.score > st2.score){
      dominance = 1f - dominance;
    }
    
    float chance = UnityEngine.Random.Range(0f, 1f);
    move_direction = (chance > dominance) ? st1.move_direction : st2.move_direction;
    
    chance = UnityEngine.Random.Range(0f, 1f);
    jump = (chance > dominance) ? st1.jump : st2.jump;

    chance = UnityEngine.Random.Range(0f, 1f);
    dash = (chance > dominance) ? st1.dash : st2.dash;

    if (jump){
      dash = false;
    }
    if (dash){
      jump = false;
    }

    chance = UnityEngine.Random.Range(0f, 1f);
    targetPlayer = (chance > dominance) ? Mathf.Lerp(st1.targetPlayer, st2.targetPlayer, 0.8f) : Mathf.Lerp(st1.targetPlayer, st2.targetPlayer, 0.2f);
    targetPlayer = Mathf.Clamp(targetPlayer, -1f, 1f);

    chance = UnityEngine.Random.Range(0f, 1f);
    targetProjectile = (chance > dominance) ? Mathf.Lerp(st1.targetProjectile, st2.targetProjectile, 0.8f) : Mathf.Lerp(st1.targetProjectile, st2.targetProjectile, 0.2f);
    targetProjectile = Mathf.Clamp(targetProjectile, 0f, 1f);

    chance = UnityEngine.Random.Range(0f, 1f);
    dashProbability = (chance > dominance) ? Mathf.Lerp(st1.dashProbability, st2.dashProbability, 0.8f) : Mathf.Lerp(st1.dashProbability, st2.dashProbability, 0.2f);
    dashProbability = Mathf.Clamp(dashProbability, 0f, 1f);

    chance = UnityEngine.Random.Range(0f, 1f);
    shoot_vector = (chance > dominance) ? Vector2.Lerp(st1.shoot_vector, st2.shoot_vector, 0.8f) : Vector2.Lerp(st1.shoot_vector, st2.shoot_vector, 0.2f);
    shoot_vector.Normalize();

    chance = UnityEngine.Random.Range(0f, 1f);
    dash_vector = (chance > dominance) ? Vector2.Lerp(st1.dash_vector, st2.dash_vector, 0.8f) : Vector2.Lerp(st1.dash_vector, st2.dash_vector, 0.2f);
    dash_vector.Normalize();

    score = (st1.score + st2.score) / 2.0f;
    score = Mathf.Clamp(score, minScore, maxScore);

    chance = UnityEngine.Random.Range(0f, 1f);
    if (chance > 0.1f){
      mutate();
    }
  }

  public void mutate(){
    move_direction = Mathf.Clamp(move_direction + UnityEngine.Random.Range(-0.1f, 0.1f), -1f, 1f);
    if (UnityEngine.Random.Range(0f, 1f) > 0.8f){
      jump = !jump;
    }
    if (UnityEngine.Random.Range(0f, 1f) > 0.8f){
      dash = !dash;
    }
    if (UnityEngine.Random.Range(0f, 1f) > 0.5f){
      targetPlayer = Mathf.Clamp(targetPlayer + UnityEngine.Random.Range(-0.2f, 0.2f), 0f, 1f);
    }
    if (UnityEngine.Random.Range(0f, 1f) > 0.5f){
      dashProbability = Mathf.Clamp(dashProbability + UnityEngine.Random.Range(-0.3f, 0.3f), 0f, 1f);
    }
    if (UnityEngine.Random.Range(0f, 1f) > 0.5f){
      targetProjectile = Mathf.Clamp(targetProjectile + UnityEngine.Random.Range(-0.3f, 0.3f), 0f, 1f);
    }
    if (UnityEngine.Random.Range(0f, 1f) > 0.8f){
      targetProjectile = -targetProjectile;
    }
    if (UnityEngine.Random.Range(0f, 1f) > 0.8f){
      targetPlayer = -targetPlayer;
    }

    if (jump){
      dash = false;
    }
    if (dash){
      jump = false;
    }

    shoot_vector = new Vector2(
      Mathf.Clamp(shoot_vector.x + UnityEngine.Random.Range(-0.05f, 0.05f), -1f, 1f),
      Mathf.Clamp(shoot_vector.y + UnityEngine.Random.Range(-0.05f, 0.05f), -1f, 1f));
    if (UnityEngine.Random.Range(0f, 1f) > 0.8f){
      shoot_vector = Vector2.zero;
    }
    
    dash_vector = new Vector2(
      Mathf.Clamp(dash_vector.x + UnityEngine.Random.Range(-0.2f, 0.2f), -1f, 1f),
      Mathf.Clamp(dash_vector.y + UnityEngine.Random.Range(-0.2f, 0.2f), -1f, 1f));
  }
}