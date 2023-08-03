package main

import (
	"context"
	"fmt"

	"github.com/google/uuid"
	"go.uber.org/zap"
	"go.uber.org/zap/zapcore"
)

func createEntity(ctx context.Context, logger *zap.Logger, client *vaultApi.Client) {
	defer wg.Done()

	data := map[string]interface{}{
		"name": fmt.Sprintf("cloudsec-org/%s", uuid.New()),
		"metadata": map[string]string{
			"buildkite_user_id":           "some static metadata",
			"buildkite_user_email":        "some static metadata",
			"buildkite_organization_slug": "some static metadata",
			"buildkite_pipeline_slug":     "some static metadata",
			"buildkite_pipeline_id":       "some static metadata",
			"buildkite_build_number":      "some static metadata",
		},
	}

	_, err := client.Logical().WriteWithContext(ctx, "identity/entity", data)
	if err != nil {
		logger.Error("Failed to create entity.", zap.Error(err))
	}
	//logger.Info("Created an entity! You go", zap.Any("resp", resp))
}

func handleCreateEntityCommand(args []string) int {
	ctx := context.Background()

	logger := vaultLogger.NewCLILogger(zapcore.InfoLevel)
	defer logger.Sync()

	client, err := vault.NewVaultClient(vaultAddress, vaultToken)
	if err != nil {
		return 1
	}

	maxGoroutines := 50
	guard := make(chan struct{}, maxGoroutines)

	desiredEntities := 100000

	for i := 1; i <= desiredEntities; i++ {
		guard <- struct{}{} // Blocks if guard channel is already full
		go func(ctx context.Context, logger *zap.Logger, client *vaultApi.Client) {
			createEntity(ctx, logger, client)
			if i%1000 == 0 {
				logger.Info("Another thousand down...", zap.Int("done", i), zap.Int("remaining", desiredEntities-i))
			}
			<-guard
		}(ctx, logger, client)
		wg.Add(1)
	}

	wg.Wait()

	return 0
}
