package com.echoshift.musyaffa.dto;

import java.util.UUID;

public class RunResponse {
    private UUID id;
    private UUID playerId;
    private Float timeElapsed;
    private Integer score;
    private Integer levelReached;

    public RunResponse(com.echoshift.musyaffa.models.Run run) {
        this.id = run.getId();
        this.playerId = run.getPlayerId();
        this.timeElapsed = run.getTimeElapsed();
        this.score = run.getScore();
        this.levelReached = run.getLevelReached();
    }

    public UUID getId() { return id; }
    public void setId(UUID id) { this.id = id; }
    
    public UUID getPlayerId() { return playerId; }
    public void setPlayerId(UUID playerId) { this.playerId = playerId; }
    
    public Float getTimeElapsed() { return timeElapsed; }
    public void setTimeElapsed(Float timeElapsed) { this.timeElapsed = timeElapsed; }
    
    public Integer getScore() { return score; }
    public void setScore(Integer score) { this.score = score; }
    
    public Integer getLevelReached() { return levelReached; }
    public void setLevelReached(Integer levelReached) { this.levelReached = levelReached; }

    @Override
    public String toString() {
        return "RunResponse{" +
                "id=" + id +
                ", playerId=" + playerId +
                ", timeElapsed=" + timeElapsed +
                ", score=" + score +
                ", levelReached=" + levelReached +
                '}';
    }
}