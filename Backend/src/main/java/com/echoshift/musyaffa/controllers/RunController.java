package com.echoshift.musyaffa.controllers;

import com.echoshift.musyaffa.models.Run;
import com.echoshift.musyaffa.models.PlayerSession;
import com.echoshift.musyaffa.repositories.RunRepository;
import com.echoshift.musyaffa.repositories.PlayerSessionRepository;
import com.echoshift.musyaffa.dto.RunRequest;
import com.echoshift.musyaffa.dto.RunResponse;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.Optional;
import java.util.UUID;
import java.util.stream.Collectors;

@RestController
@RequestMapping("/api/runs")
public class RunController {

    @Autowired
    private RunRepository runRepository;
    
    @Autowired
    private PlayerSessionRepository playerSessionRepository;

    @GetMapping("/{playerId}")
    public ResponseEntity<BaseResponse<List<RunResponse>>> getRunsByPlayerId(@PathVariable UUID playerId) {
        try {
            System.out.println("=== GET RUNS BY PLAYER DEBUG START ===");
            System.out.println("Fetching runs for player ID: " + playerId);
            
            List<Run> runs = runRepository.findByPlayerIdOrderByScoreDesc(playerId);
            System.out.println("Found " + runs.size() + " runs for player " + playerId + " (sorted by highest score)");
            
            List<RunResponse> runResponses = runs.stream()
                    .map(RunResponse::new)
                    .collect(Collectors.toList());
            
            System.out.println("Converted to " + runResponses.size() + " response DTOs");
            
            BaseResponse<List<RunResponse>> response = new BaseResponse<>(true, "Player runs retrieved successfully", runResponses);
            System.out.println("=== GET RUNS BY PLAYER DEBUG END (SUCCESS) ===");
            return ResponseEntity.ok(response);
        } catch (Exception e) {
            System.err.println("=== GET RUNS BY PLAYER ERROR ===");
            System.err.println("ERROR in getRunsByPlayerId: " + e.getMessage());
            e.printStackTrace();
            BaseResponse<List<RunResponse>> response = new BaseResponse<>(false, "Error retrieving player runs: " + e.getMessage(), null);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(response);
        }
    }

    @PostMapping
    public ResponseEntity<BaseResponse<RunResponse>> createRun(
            @RequestBody RunRequest request,
            @RequestHeader("Authorization") String sessionToken) {
        
        System.out.println("=== CREATE RUN DEBUG START ===");
        System.out.println("Received createRun request: " + request);
        System.out.println("Session token: " + (sessionToken != null ? sessionToken.substring(0, Math.min(10, sessionToken.length())) + "..." : "null"));
        
        try {
            // Validate session
            System.out.println("Looking for session with token: " + sessionToken);
            Optional<PlayerSession> sessionOptional = playerSessionRepository.findBySessionTokenAndIsActive(sessionToken, true);
            
            if (!sessionOptional.isPresent()) {
                System.out.println("ERROR: Session not found or inactive");
                BaseResponse<RunResponse> response = new BaseResponse<>(false, "Invalid session", null);
                return ResponseEntity.status(HttpStatus.UNAUTHORIZED).body(response);
            }
            
            PlayerSession session = sessionOptional.get();
            UUID playerId = session.getPlayer().getId();
            System.out.println("Session validated. Player ID: " + playerId);
            
            // Log the request data in detail
            System.out.println("Request details:");
            System.out.println("  - Score: " + request.getScore());
            System.out.println("  - TimeElapsed: " + request.getTimeElapsed());
            System.out.println("  - LevelReached: " + request.getLevelReached());
            
            // Create run with only the fields that exist in database
            Run run = new Run();
            run.setPlayerId(playerId);
            run.setTimeElapsed(request.getTimeElapsed() != null ? request.getTimeElapsed() : 0.0f);
            run.setScore(request.getScore() != null ? request.getScore() : 0);
            run.setLevelReached(request.getLevelReached() != null ? request.getLevelReached() : 1);
            
            System.out.println("Created Run object:");
            System.out.println("  - Player ID: " + run.getPlayerId());
            System.out.println("  - Score: " + run.getScore());
            System.out.println("  - TimeElapsed: " + run.getTimeElapsed());
            System.out.println("  - LevelReached: " + run.getLevelReached());
            
            System.out.println("Saving run to database...");
            Run savedRun = runRepository.save(run);
            System.out.println("Run saved successfully with ID: " + savedRun.getId());
            
            // Log saved run details
            System.out.println("Saved Run details:");
            System.out.println("  - ID: " + savedRun.getId());
            System.out.println("  - Player ID: " + savedRun.getPlayerId());
            System.out.println("  - Score: " + savedRun.getScore());
            System.out.println("  - TimeElapsed: " + savedRun.getTimeElapsed());
            System.out.println("  - LevelReached: " + savedRun.getLevelReached());
            
            // Convert to DTO to avoid proxy issues
            RunResponse responseDto = new RunResponse(savedRun);
            System.out.println("Created response DTO:");
            System.out.println("  - ID: " + responseDto.getId());
            System.out.println("  - Player ID: " + responseDto.getPlayerId());
            System.out.println("  - Score: " + responseDto.getScore());
            System.out.println("  - TimeElapsed: " + responseDto.getTimeElapsed());
            System.out.println("  - LevelReached: " + responseDto.getLevelReached());
            
            BaseResponse<RunResponse> response = new BaseResponse<>(true, "Run created successfully", responseDto);
            System.out.println("=== CREATE RUN DEBUG END (SUCCESS) ===");
            return ResponseEntity.status(HttpStatus.CREATED).body(response);
            
        } catch (Exception e) {
            System.err.println("=== CREATE RUN ERROR ===");
            System.err.println("ERROR in createRun: " + e.getMessage());
            System.err.println("Error class: " + e.getClass().getSimpleName());
            e.printStackTrace();
            BaseResponse<RunResponse> response = new BaseResponse<>(false, "Error creating run: " + e.getMessage(), null);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(response);
        }
    }

    @PutMapping("/{id}")
    public ResponseEntity<BaseResponse<RunResponse>> updateRun(@PathVariable UUID id, @RequestBody RunRequest updateData) {
        System.out.println("=== UPDATE RUN DEBUG START ===");
        System.out.println("Updating run ID: " + id);
        System.out.println("Update data: " + updateData);
        System.out.println("Update details:");
        System.out.println("  - Score: " + updateData.getScore());
        System.out.println("  - TimeElapsed: " + updateData.getTimeElapsed());
        System.out.println("  - LevelReached: " + updateData.getLevelReached());
        
        try {
            System.out.println("Looking for run with ID: " + id);
            Optional<Run> runOptional = runRepository.findById(id);
            if (runOptional.isPresent()) {
                Run run = runOptional.get();
                System.out.println("Found run. Current values:");
                System.out.println("  - Current Score: " + run.getScore());
                System.out.println("  - Current TimeElapsed: " + run.getTimeElapsed());
                System.out.println("  - Current LevelReached: " + run.getLevelReached());
                
                if (updateData.getScore() != null) {
                    System.out.println("Updating score from " + run.getScore() + " to " + updateData.getScore());
                    run.setScore(updateData.getScore());
                }
                if (updateData.getTimeElapsed() != null) {
                    System.out.println("Updating timeElapsed from " + run.getTimeElapsed() + " to " + updateData.getTimeElapsed());
                    run.setTimeElapsed(updateData.getTimeElapsed());
                }
                if (updateData.getLevelReached() != null) {
                    System.out.println("Updating levelReached from " + run.getLevelReached() + " to " + updateData.getLevelReached());
                    run.setLevelReached(updateData.getLevelReached());
                }
                
                System.out.println("About to save updated run...");
                Run updatedRun = runRepository.save(run);
                
                System.out.println("Run updated successfully. Final values:");
                System.out.println("  - Final Score: " + updatedRun.getScore());
                System.out.println("  - Final TimeElapsed: " + updatedRun.getTimeElapsed());
                System.out.println("  - Final LevelReached: " + updatedRun.getLevelReached());
                
                // Convert to DTO to avoid proxy issues
                RunResponse responseDto = new RunResponse(updatedRun);
                System.out.println("Created response DTO with values:");
                System.out.println("  - DTO Score: " + responseDto.getScore());
                System.out.println("  - DTO TimeElapsed: " + responseDto.getTimeElapsed());
                System.out.println("  - DTO LevelReached: " + responseDto.getLevelReached());
                
                BaseResponse<RunResponse> response = new BaseResponse<>(true, "Run updated successfully", responseDto);
                System.out.println("=== UPDATE RUN DEBUG END (SUCCESS) ===");
                return ResponseEntity.ok(response);
            } else {
                System.out.println("ERROR: Run not found with ID: " + id);
                BaseResponse<RunResponse> response = new BaseResponse<>(false, "Run not found", null);
                return ResponseEntity.status(HttpStatus.NOT_FOUND).body(response);
            }
        } catch (Exception e) {
            System.err.println("=== UPDATE RUN ERROR ===");
            System.err.println("ERROR in updateRun: " + e.getMessage());
            System.err.println("Error class: " + e.getClass().getSimpleName());
            e.printStackTrace();
            BaseResponse<RunResponse> response = new BaseResponse<>(false, "Error updating run: " + e.getMessage(), null);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(response);
        }
    }

    @PatchMapping("/{id}/end")
    public ResponseEntity<BaseResponse<RunResponse>> endRun(@PathVariable UUID id, @RequestBody RunRequest endRunData) {
        System.out.println("=== END RUN DEBUG START ===");
        System.out.println("Ending run ID: " + id);
        System.out.println("End run data: " + endRunData);
        System.out.println("End run details:");
        System.out.println("  - Final Score: " + endRunData.getScore());
        System.out.println("  - Final TimeElapsed: " + endRunData.getTimeElapsed());
        System.out.println("  - Final LevelReached: " + endRunData.getLevelReached());
        
        try {
            System.out.println("Looking for run with ID: " + id);
            Optional<Run> runOptional = runRepository.findById(id);
            if (runOptional.isPresent()) {
                Run run = runOptional.get();
                System.out.println("Found run. Current values before ending:");
                System.out.println("  - Current Score: " + run.getScore());
                System.out.println("  - Current TimeElapsed: " + run.getTimeElapsed());
                System.out.println("  - Current LevelReached: " + run.getLevelReached());
                            
                if (endRunData.getScore() != null) {
                    System.out.println("Setting final score from " + run.getScore() + " to " + endRunData.getScore());
                    run.setScore(endRunData.getScore());
                }
                if (endRunData.getTimeElapsed() != null) {
                    System.out.println("Setting final timeElapsed from " + run.getTimeElapsed() + " to " + endRunData.getTimeElapsed());
                    run.setTimeElapsed(endRunData.getTimeElapsed());
                }
                if (endRunData.getLevelReached() != null) {
                    System.out.println("Setting final levelReached from " + run.getLevelReached() + " to " + endRunData.getLevelReached());
                    run.setLevelReached(endRunData.getLevelReached());
                }
                
                System.out.println("About to save final run state...");
                Run updatedRun = runRepository.save(run);
                
                System.out.println("Run ended successfully. Final saved values:");
                System.out.println("  - Final Score: " + updatedRun.getScore());
                System.out.println("  - Final TimeElapsed: " + updatedRun.getTimeElapsed());
                System.out.println("  - Final LevelReached: " + updatedRun.getLevelReached());
                
                RunResponse responseDto = new RunResponse(updatedRun);
                System.out.println("Created final response DTO:");
                System.out.println("  - DTO Score: " + responseDto.getScore());
                System.out.println("  - DTO TimeElapsed: " + responseDto.getTimeElapsed());
                System.out.println("  - DTO LevelReached: " + responseDto.getLevelReached());
                
                BaseResponse<RunResponse> response = new BaseResponse<>(true, "Run completed successfully", responseDto);
                System.out.println("=== END RUN DEBUG END (SUCCESS) ===");
                return ResponseEntity.ok(response);
            } else {
                System.out.println("ERROR: Run not found with ID: " + id);
                BaseResponse<RunResponse> response = new BaseResponse<>(false, "Run not found", null);
                return ResponseEntity.status(HttpStatus.NOT_FOUND).body(response);
            }
        } catch (Exception e) {
            System.err.println("=== END RUN ERROR ===");
            System.err.println("ERROR in endRun: " + e.getMessage());
            System.err.println("Error class: " + e.getClass().getSimpleName());
            e.printStackTrace();
            BaseResponse<RunResponse> response = new BaseResponse<>(false, "Error ending run: " + e.getMessage(), null);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(response);
        }
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<BaseResponse<Void>> deleteRun(
            @PathVariable UUID id,
            @RequestHeader("Authorization") String sessionToken) {
        
        try {
            // Validate session
            Optional<PlayerSession> sessionOptional = playerSessionRepository.findBySessionTokenAndIsActive(sessionToken, true);
            
            if (!sessionOptional.isPresent()) {
                BaseResponse<Void> response = new BaseResponse<>(false, "Invalid session", null);
                return ResponseEntity.status(HttpStatus.UNAUTHORIZED).body(response);
            }
            
            PlayerSession session = sessionOptional.get();
            UUID currentPlayerId = session.getPlayer().getId();
            
            // Check if run exists and belongs to the current player
            Optional<Run> runOptional = runRepository.findById(id);
            if (!runOptional.isPresent()) {
                BaseResponse<Void> response = new BaseResponse<>(false, "Run not found", null);
                return ResponseEntity.status(HttpStatus.NOT_FOUND).body(response);
            }
            
            Run run = runOptional.get();
            if (!run.getPlayerId().equals(currentPlayerId)) {
                BaseResponse<Void> response = new BaseResponse<>(false, "You can only delete your own runs", null);
                return ResponseEntity.status(HttpStatus.FORBIDDEN).body(response);
            }
            
            // Delete the run
            runRepository.deleteById(id);
            BaseResponse<Void> response = new BaseResponse<>(true, "Run deleted successfully", null);
            return ResponseEntity.ok(response);
            
        } catch (Exception e) {
            BaseResponse<Void> response = new BaseResponse<>(false, "Error deleting run: " + e.getMessage(), null);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(response);
        }
    }
}
