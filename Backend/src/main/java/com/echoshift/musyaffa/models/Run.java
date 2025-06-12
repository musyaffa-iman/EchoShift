package com.echoshift.musyaffa.models;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import jakarta.persistence.*;
import java.util.UUID;

@Entity
@Table(name = "runs")
@JsonIgnoreProperties({"hibernateLazyInitializer", "handler"})
public class Run extends BaseEntity {
    
    @Column(name = "player_id", nullable = false)
    @JsonProperty("player_id")
    private UUID playerId;

    @Column(name = "time_elapsed", nullable = false, columnDefinition = "FLOAT DEFAULT 0.0")
    @JsonProperty("time_elapsed")
    private Float timeElapsed;

    @Column(name = "score", columnDefinition = "INT DEFAULT 0")
    private Integer score;

    @Column(name = "level_reached", columnDefinition = "INT DEFAULT 1")
    @JsonProperty("level_reached")
    private Integer levelReached;

    // Constructors
    public Run() {}

    public Run(UUID playerId, Float timeElapsed, Integer score, Integer levelReached) {
        this.playerId = playerId;
        this.timeElapsed = timeElapsed;
        this.score = score;
        this.levelReached = levelReached;
    }

    // Getters and Setters
    public UUID getPlayerId() {
        return playerId;
    }

    public void setPlayerId(UUID playerId) {
        this.playerId = playerId;
    }

    public Float getTimeElapsed() {
        return timeElapsed;
    }

    public void setTimeElapsed(Float timeElapsed) {
        this.timeElapsed = timeElapsed;
    }

    public Integer getScore() {
        return score;
    }

    public void setScore(Integer score) {
        this.score = score;
    }

    public Integer getLevelReached() {
        return levelReached;
    }

    public void setLevelReached(Integer levelReached) {
        this.levelReached = levelReached;
    }
}
