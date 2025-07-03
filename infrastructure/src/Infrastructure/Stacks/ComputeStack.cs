using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.CustomResources;
using Constructs;
using Infrastructure.Config;
using Infrastructure.Utils;

namespace Infrastructure.Stacks;

public class ComputeStack : Stack
{
    public CfnOutput LightsailInstanceId { get; }

    public ComputeStack(Construct scope, string id, EnvironmentConfig config, IStackProps? props = null) 
        : base(scope, id, props)
    {
        // Create custom resource for Lightsail instance
        var customResource = new CfnCustomResource(this, Naming.CreateResourceName("LightsailInstance", config.Environment), new CfnCustomResourceProps
        {
            ServiceToken = CreateLightsailLambda().FunctionArn
        });

        // Set properties using the correct approach
        customResource.AddPropertyOverride("InstanceName", Naming.CreateResourceName("lightsail-instance", config.Environment));
        customResource.AddPropertyOverride("BlueprintId", "amazon_linux_2");
        customResource.AddPropertyOverride("BundleId", "nano_2_0");
        customResource.AddPropertyOverride("AvailabilityZone", config.AvailabilityZone);
        customResource.AddPropertyOverride("UserData", GetUserData());

        LightsailInstanceId = new CfnOutput(this, "LightsailInstanceId", new CfnOutputProps
        {
            Value = customResource.Ref,
            Description = "Lightsail Instance ID"
        });

        // Add tags
        TagUtils.AddCommonTags(customResource, config.Environment);
        TagUtils.AddPurposeTag(customResource, "Compute");
        TagUtils.AddCostCenterTag(customResource);
    }

    private Function CreateLightsailLambda()
    {
        return new Function(this, "LightsailLambda", new FunctionProps
        {
            Runtime = Runtime.NODEJS_18_X,
            Handler = "index.handler",
            Code = Code.FromInline(@"
const AWS = require('aws-sdk');
const lightsail = new AWS.Lightsail();

exports.handler = async (event) => {
    const { RequestType, ResourceProperties } = event;
    const { InstanceName, BlueprintId, BundleId, AvailabilityZone, UserData } = ResourceProperties;
    
    try {
        if (RequestType === 'Create' || RequestType === 'Update') {
            const params = {
                instanceNames: [InstanceName],
                availabilityZone: AvailabilityZone,
                blueprintId: BlueprintId,
                bundleId: BundleId,
                userData: UserData
            };
            
            const result = await lightsail.createInstances(params).promise();
            return {
                PhysicalResourceId: InstanceName,
                Data: {
                    InstanceId: result.operations[0].id
                }
            };
        } else if (RequestType === 'Delete') {
            await lightsail.deleteInstance({ instanceName: InstanceName }).promise();
            return { PhysicalResourceId: InstanceName };
        }
    } catch (error) {
        console.error('Error:', error);
        throw error;
    }
};
            "),
            Timeout = Duration.Minutes(5)
        });
    }

    private string GetUserData()
    {
        return @"#!/bin/bash
yum update -y
yum install -y docker
systemctl start docker
systemctl enable docker
usermod -a -G docker ec2-user
yum install -y git
";
    }
} 