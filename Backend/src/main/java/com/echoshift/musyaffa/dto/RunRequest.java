package com.echoshift.musyaffa.dto;

public class RunRequest {
    private Integer score;
    private Float timeElapsed;
    private Integer levelReached;

    public RunRequest() {
    }

    public RunRequest(Integer score, Float timeElapsed, Integer levelReached) {
        this.score = score;
        this.timeElapsed = timeElapsed;
        this.levelReached = levelReached;
    }

    public Integer getScore() {
        return score;
    }

    public void setScore(Integer score) {
        this.score = score;
    }

    public Float getTimeElapsed() {
        return timeElapsed;
    }

    public void setTimeElapsed(Float timeElapsed) {
        this.timeElapsed = timeElapsed;
    }

    public Integer getLevelReached() {
        return levelReached;
    }

    public void setLevelReached(Integer levelReached) {
        this.levelReached = levelReached;
    }

    @Override
    public String toString() {
        return "RunRequest{" +
                "score=" + score +
                ", timeElapsed=" + timeElapsed +
                ", levelReached=" + levelReached +
                '}';
    }
}
