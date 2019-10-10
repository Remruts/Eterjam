using System;
using UnityEngine;

public class CustomTimer {

  float currentTime;
  float maxTime;
  float tickScale = 1.0f;
  Action actionToExecute;

  public CustomTimer(float duration, Action theAction){
    maxTime = duration;
    actionToExecute = theAction;
    currentTime = maxTime;
  }

  public void tick(Func<float> resetExpression, bool resetCondition = true, bool executionCondition = true){
    currentTime -= Time.deltaTime * tickScale;
    if (currentTime <= 0f){
      if (resetCondition){
        currentTime = resetExpression();
      }
      if (executionCondition){
        actionToExecute();
      }
    }
  }

  public float getCurrentTime(){
    return currentTime;
  }

  public float getDuration(){
    return maxTime;
  }

  public void setDuration(float time){
    maxTime = time;
  }

  
  public void setTickScale(float ts){
    tickScale = ts;
  }

  public float getTickScale(){
    return tickScale;
  }

  // Un reset sin side effects
  public void resetTimerInstantly(){
    currentTime = maxTime;
  }

  public void start(float time){
    setDuration(time);
    resetTimerInstantly();
  }

  public void setAction(Action theAction){
    actionToExecute = theAction;
  }
}

public class Angle360{

  float myAngle;
  public Angle360(float angle, bool radians = false){
    myAngle = angle;
    if (radians){
      myAngle *= Mathf.Rad2Deg;
    }
    
    recalculate();
  }

  public static Angle360 operator+ (Angle360 a, Angle360 b)
    => new Angle360(a.myAngle + b.myAngle);

  public static Angle360 operator* (Angle360 a, Angle360 b)
    => new Angle360(a.myAngle * b.myAngle);

  public static bool operator== (Angle360 a, Angle360 b)
    => Mathf.Approximately(a.myAngle, b.myAngle);

  public static bool operator!= (Angle360 a, Angle360 b)
    => !(a == b);

  public override bool Equals(object obj){
    if (obj == null || GetType() != obj.GetType()){
        return false;
    }
   
    return base.Equals (obj);
  }
  
  public override int GetHashCode()
  {
    // TODO: write your implementation of GetHashCode() here
    throw new System.NotImplementedException();
    //return myAngle.GetHashCode();
  }
  

  public static bool operator >(Angle360 a, Angle360 b)
    => (a.myAngle > b.myAngle);
  

  public static bool operator <(Angle360 a, Angle360 b)
    => (a.myAngle < b.myAngle);
  

  public static bool operator >=(Angle360 a, Angle360 b)
    => (a.myAngle >= b.myAngle);

  public static bool operator <=(Angle360 a, Angle360 b) 
    => (a.myAngle <= b.myAngle);  

  public static implicit operator float(Angle360 a) 
    => a.myAngle;
  public static explicit operator Angle360(float a) 
    => new Angle360(a);

  public override string ToString() => $"{myAngle}";

  public static Angle360 lerp(float startAngle, float endAngle, float rotationSpeed = 0.1f){
    float speed = Mathf.Max(Mathf.Min(1f, rotationSpeed), 0f);

    startAngle = new Angle360(startAngle);
    endAngle = new Angle360(endAngle);
    
    float shortest_angle = ((((endAngle - startAngle) % 360) + 540) % 360) - 180;
    startAngle += (shortest_angle * speed) % 360;

    return new Angle360(startAngle);
  }

  public void add(float angle){
    myAngle += angle;
    recalculate();
  }

  public void setAngle(float newAngle, bool radians = false){
    myAngle = newAngle * (radians ? Mathf.Rad2Deg : 1f);
    recalculate();
  }

  public float getAngle(){
    return myAngle;
  }

  public float getAngleRad(){
    return myAngle * Mathf.Deg2Rad;
  }

  public void lerpTo(float endAngle, float rotationSpeed = 0.1f){
    float speed = Mathf.Max(Mathf.Min(1f, rotationSpeed), 0f);
    
    float shortest_angle = ((((endAngle - myAngle) % 360) + 540) % 360) - 180;
    myAngle += (shortest_angle * speed) % 360;
    recalculate();
  }

  void recalculate(){
    myAngle = (myAngle % 360 + 360) % 360;
  }
}